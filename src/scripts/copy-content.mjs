import { cp, mkdir, readdir, readFile, rm, stat, writeFile } from 'node:fs/promises'
import path from 'node:path'

const appRoot = process.cwd()
const repoRoot = path.resolve(appRoot, '..')
const destination = path.join(appRoot, 'public', 'content')

const allowedExtensions = new Set([
  '.md', '.kql', '.csv', '.json', '.http', '.ps1', '.cs', '.csproj', '.yml', '.yaml'
])

await rm(destination, { recursive: true, force: true })
await mkdir(destination, { recursive: true })

const sources = [
  ['docs', path.join(repoRoot, 'docs')],
  ['examples', path.join(repoRoot, 'examples')],
  ['samples/dotnet-golden-agent', path.join(repoRoot, 'samples', 'dotnet-golden-agent')]
]

const entries = []

async function copyTree(prefix, sourceDir) {
  const children = await readdir(sourceDir, { withFileTypes: true })

  for (const child of children) {
    const sourcePath = path.join(sourceDir, child.name)
    const relativePath = path.posix.join(prefix, child.name.replaceAll('\\', '/'))

    if (child.isDirectory()) {
      if (['bin', 'obj', 'publish', 'node_modules'].includes(child.name)) continue
      await copyTree(relativePath, sourcePath)
      continue
    }

    const ext = path.extname(child.name).toLowerCase()
    if (!allowedExtensions.has(ext)) continue

    const targetPath = path.join(destination, ...relativePath.split('/'))
    await mkdir(path.dirname(targetPath), { recursive: true })
    await cp(sourcePath, targetPath)

    const raw = await readFile(sourcePath, 'utf8')
    const firstHeading = raw.match(/^#\s+(.+)$/m)?.[1]?.trim()

    entries.push({
      path: relativePath,
      title: firstHeading || child.name,
      kind: ext === '.md' ? 'markdown' : 'code',
      language: relativePath.startsWith('docs/en/') ? 'en' : relativePath.startsWith('docs/') ? 'it' : 'shared'
    })
  }
}

for (const [prefix, source] of sources) {
  try {
    if ((await stat(source)).isDirectory()) {
      await copyTree(prefix, source)
    }
  } catch {
    // Optional source folder: skip when not present.
  }
}

const rootReadme = path.join(repoRoot, 'README.md')
try {
  await cp(rootReadme, path.join(destination, 'README.md'))
  entries.unshift({
    path: 'README.md',
    title: 'Agent 365 Knowledge Hub',
    kind: 'markdown',
    language: 'shared'
  })
} catch {
  // Root README is optional during isolated app development.
}

entries.sort((a, b) => a.path.localeCompare(b.path))
await writeFile(
  path.join(destination, 'index.json'),
  JSON.stringify({ generatedAt: new Date().toISOString(), entries }, null, 2)
)

console.log(`Synced ${entries.length} knowledge-base files into public/content`)
