import { useEffect, useMemo, useState } from 'react'
import ReactMarkdown from 'react-markdown'
import remarkGfm from 'remark-gfm'

function resolveRelative(currentPath, href) {
  if (!href || href.startsWith('#')) return null
  if (/^https?:\/\//i.test(href)) return null

  const base = new URL(currentPath, 'https://knowledge.local/')
  const resolved = new URL(href, base)
  return decodeURIComponent(resolved.pathname.replace(/^\//, ''))
}

function languageForPath(path) {
  if (path.startsWith('docs/en/')) return 'en'
  if (path.startsWith('docs/')) return 'it'
  return 'shared'
}

function groupLabel(path, lang) {
  if (path === 'README.md') return lang === 'it' ? 'Repository' : 'Repository'
  if (path.includes('/tutorials/')) return lang === 'it' ? 'Tutorial pratici' : 'Hands-on tutorials'
  if (path.startsWith('docs/en/')) return 'Academy EN'
  if (path.startsWith('docs/')) return 'Academy IT'
  if (path.startsWith('examples/')) return lang === 'it' ? 'Asset & esempi' : 'Assets & examples'
  if (path.startsWith('samples/')) return lang === 'it' ? 'Golden sample' : 'Golden sample'
  return lang === 'it' ? 'Altro' : 'Other'
}

function codeLanguage(path) {
  const ext = path.split('.').pop()?.toLowerCase()
  return ({
    kql: 'kusto',
    csv: 'csv',
    json: 'json',
    http: 'http',
    ps1: 'powershell',
    cs: 'csharp',
    csproj: 'xml',
    yml: 'yaml',
    yaml: 'yaml'
  })[ext] || 'text'
}

export default function MarkdownViewer({ lang, requestedPath, onPathChange }) {
  const [index, setIndex] = useState([])
  const [selectedPath, setSelectedPath] = useState(requestedPath || (lang === 'it' ? 'docs/README.md' : 'docs/en/README.md'))
  const [body, setBody] = useState('')
  const [kind, setKind] = useState('markdown')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const baseUrl = `${import.meta.env.BASE_URL}content/`

  useEffect(() => {
    fetch(`${baseUrl}index.json`)
      .then(r => {
        if (!r.ok) throw new Error(`HTTP ${r.status}`)
        return r.json()
      })
      .then(data => setIndex(data.entries || []))
      .catch(err => setError(`Knowledge index unavailable: ${err.message}`))
  }, [baseUrl])

  useEffect(() => {
    if (requestedPath && requestedPath !== selectedPath) {
      setSelectedPath(requestedPath)
    }
  }, [requestedPath, selectedPath])

  useEffect(() => {
    if (!requestedPath) {
      const currentLanguage = languageForPath(selectedPath)
      if (currentLanguage === 'it' && lang === 'en') {
        setSelectedPath('docs/en/README.md')
      } else if (currentLanguage === 'en' && lang === 'it') {
        setSelectedPath('docs/README.md')
      }
    }
  }, [lang, requestedPath, selectedPath])

  useEffect(() => {
    if (!selectedPath) return

    setLoading(true)
    setError('')

    const entry = index.find(x => x.path === selectedPath)
    setKind(entry?.kind || (selectedPath.endsWith('.md') ? 'markdown' : 'code'))

    fetch(`${baseUrl}${selectedPath.split('/').map(encodeURIComponent).join('/')}`)
      .then(r => {
        if (!r.ok) throw new Error(`HTTP ${r.status}`)
        return r.text()
      })
      .then(setBody)
      .catch(err => {
        setBody('')
        setError(`Unable to load ${selectedPath}: ${err.message}`)
      })
      .finally(() => setLoading(false))
  }, [baseUrl, index, selectedPath])

  const visibleEntries = useMemo(() => {
    return index.filter(entry => {
      if (entry.language === 'shared') return true
      return entry.language === lang
    })
  }, [index, lang])

  const groups = useMemo(() => {
    const grouped = new Map()
    for (const entry of visibleEntries) {
      const label = groupLabel(entry.path, lang)
      if (!grouped.has(label)) grouped.set(label, [])
      grouped.get(label).push(entry)
    }
    return [...grouped.entries()]
  }, [visibleEntries, lang])

  const navigate = path => {
    if (!path) return
    setSelectedPath(path)
    onPathChange?.(path)
  }

  const markdown = kind === 'markdown'
    ? body
    : `\`\`\`${codeLanguage(selectedPath)}\n${body}\n\`\`\``

  return (
    <div className="kb-shell">
      <aside className="kb-sidebar">
        <div className="kb-sidebar-head">
          <span>{lang === 'it' ? 'CONTENUTI' : 'CONTENTS'}</span>
          <strong>{visibleEntries.length}</strong>
        </div>
        <div className="kb-nav">
          {groups.map(([label, entries]) => (
            <div className="kb-group" key={label}>
              <p>{label}</p>
              {entries.map(entry => (
                <button
                  className={entry.path === selectedPath ? 'active' : ''}
                  key={entry.path}
                  onClick={() => navigate(entry.path)}
                  title={entry.path}
                >
                  <span>{entry.kind === 'markdown' ? '¶' : '⌘'}</span>
                  {entry.title}
                </button>
              ))}
            </div>
          ))}
        </div>
      </aside>

      <article className="kb-viewer">
        <div className="kb-toolbar">
          <div>
            <span>{kind === 'markdown' ? 'MARKDOWN' : 'SOURCE'}</span>
            <code>{selectedPath}</code>
          </div>
          <a
            href={`https://github.com/KeyserDSoze/Agent365/blob/main/${selectedPath}`}
            target="_blank"
            rel="noreferrer"
          >
            GitHub ↗
          </a>
        </div>

        <div className="markdown-body">
          {loading && <p className="kb-state">{lang === 'it' ? 'Caricamento…' : 'Loading…'}</p>}
          {error && <p className="kb-error">{error}</p>}
          {!loading && !error && (
            <ReactMarkdown
              remarkPlugins={[remarkGfm]}
              components={{
                a({ href, children, ...props }) {
                  const internal = resolveRelative(selectedPath, href)
                  const isKnowledgeFile = internal && index.some(x => x.path === internal)

                  if (isKnowledgeFile) {
                    return (
                      <a
                        href={href}
                        onClick={event => {
                          event.preventDefault()
                          navigate(internal)
                        }}
                        {...props}
                      >
                        {children}
                      </a>
                    )
                  }

                  return (
                    <a href={href} target="_blank" rel="noreferrer" {...props}>
                      {children}
                    </a>
                  )
                }
              }}
            >
              {markdown}
            </ReactMarkdown>
          )}
        </div>
      </article>
    </div>
  )
}
