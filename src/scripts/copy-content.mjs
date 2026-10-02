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
  ['samples/dotnet-golden-agent', path.join(repoRoot, 'samples', 'dotnet-golden-agent')],
  ['samples/mcp-governed-tools', path.join(repoRoot, 'samples', 'mcp-governed-tools')]
]

const entries = []

function normalizeSearchText(raw) {
  return raw
    .replace(/```[\s\S]*?```/g, ' ')
    .replace(/<[^>]+>/g, ' ')
    .replace(/[#>*_[\](){}|~]/g, ' ')
    .replace(/\s+/g, ' ')
    .trim()
}

function makeIndexEntry(relativePath, ext, raw, fallbackTitle) {
  const firstHeading = raw.match(/^#\s+(.+)$/m)?.[1]?.trim()
  const normalized = normalizeSearchText(raw)
  const title = firstHeading || fallbackTitle

  return {
    path: relativePath,
    title,
    kind: ext === '.md' ? 'markdown' : 'code',
    language: relativePath.startsWith('docs/en/')
      ? 'en'
      : relativePath.startsWith('docs/')
        ? 'it'
        : 'shared',
    excerpt: normalized.slice(0, 240),
    search: `${title} ${relativePath} ${normalized.slice(0, 16000)}`.toLowerCase()
  }
}

async function copyTree(prefix, sourceDir) {
  const children = await readdir(sourceDir, { withFileTypes: true })

  for (const child of children) {
    const sourcePath = path.join(sourceDir, child.name)
    const relativePath = path.posix.join(prefix, child.name.replaceAll('\\', '/'))

    if (child.isDirectory()) {
      if (['bin', 'obj', 'publish', 'node_modules', 'lab-output'].includes(child.name)) continue
      await copyTree(relativePath, sourcePath)
      continue
    }

    const ext = path.extname(child.name).toLowerCase()
    if (!allowedExtensions.has(ext)) continue

    const targetPath = path.join(destination, ...relativePath.split('/'))
    await mkdir(path.dirname(targetPath), { recursive: true })
    await cp(sourcePath, targetPath)

    const raw = await readFile(sourcePath, 'utf8')
    entries.push(makeIndexEntry(relativePath, ext, raw, child.name))
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
  const raw = await readFile(rootReadme, 'utf8')
  await cp(rootReadme, path.join(destination, 'README.md'))
  entries.unshift(makeIndexEntry('README.md', '.md', raw, 'Agent 365 Knowledge Hub'))
} catch {
  // Root README is optional during isolated app development.
}

entries.sort((a, b) => a.path.localeCompare(b.path))

await writeFile(
  path.join(destination, 'index.json'),
  JSON.stringify(
    {
      generatedAt: new Date().toISOString(),
      files: entries.length,
      entries
    },
    null,
    2
  )
)

console.log(`Synced ${entries.length} searchable knowledge-base files into public/content`)
