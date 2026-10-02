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
  if (path === 'README.md') return 'Repository'
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

function slugify(value) {
  return String(value ?? '')
    .toLowerCase()
    .normalize('NFKD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/[^a-z0-9\s-]/g, '')
    .trim()
    .replace(/\s+/g, '-')
    .replace(/-+/g, '-')
}

function headingText(children) {
  if (Array.isArray(children)) return children.map(headingText).join('')
  if (typeof children === 'string' || typeof children === 'number') return String(children)
  return ''
}

export default function MarkdownViewer({ lang, requestedPath, onPathChange }) {
  const [index, setIndex] = useState([])
  const [selectedPath, setSelectedPath] = useState(
    requestedPath || (lang === 'it' ? 'docs/README.md' : 'docs/en/README.md')
  )
  const [body, setBody] = useState('')
  const [kind, setKind] = useState('markdown')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [search, setSearch] = useState('')
  const [copied, setCopied] = useState(false)

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
    setCopied(false)

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

  const languageEntries = useMemo(() => {
    return index.filter(entry => entry.language === 'shared' || entry.language === lang)
  }, [index, lang])

  const visibleEntries = useMemo(() => {
    const q = search.trim().toLowerCase()
    if (!q) return languageEntries

    const terms = q.split(/\s+/).filter(Boolean)

    return languageEntries
      .map(entry => {
        const haystack = entry.search || `${entry.title} ${entry.path} ${entry.excerpt || ''}`.toLowerCase()
        const score = terms.reduce((acc, term) => {
          if (entry.title.toLowerCase().includes(term)) return acc + 6
          if (entry.path.toLowerCase().includes(term)) return acc + 3
          if (haystack.includes(term)) return acc + 1
          return -999
        }, 0)

        return { entry, score }
      })
      .filter(x => x.score >= 0)
      .sort((a, b) => b.score - a.score || a.entry.path.localeCompare(b.entry.path))
      .map(x => x.entry)
  }, [languageEntries, search])

  const groups = useMemo(() => {
    const grouped = new Map()
    for (const entry of visibleEntries) {
      const label = groupLabel(entry.path, lang)
      if (!grouped.has(label)) grouped.set(label, [])
      grouped.get(label).push(entry)
    }
    return [...grouped.entries()]
  }, [visibleEntries, lang])

  const currentIndex = languageEntries.findIndex(entry => entry.path === selectedPath)
  const previous = currentIndex > 0 ? languageEntries[currentIndex - 1] : null
  const next = currentIndex >= 0 && currentIndex < languageEntries.length - 1
    ? languageEntries[currentIndex + 1]
    : null

  const navigate = path => {
    if (!path) return
    setSelectedPath(path)
    onPathChange?.(path)
  }

  const copyLink = async () => {
    try {
      await navigator.clipboard.writeText(window.location.href)
      setCopied(true)
      window.setTimeout(() => setCopied(false), 1800)
    } catch {
      setCopied(false)
    }
  }

  const scrollToHeading = hash => {
    const id = decodeURIComponent(hash.replace(/^#/, ''))
    const element = document.getElementById(id)
    element?.scrollIntoView({ behavior: 'smooth', block: 'start' })
  }

  const heading = Tag => ({ children, ...props }) => {
    const id = slugify(headingText(children))
    return <Tag id={id} {...props}>{children}</Tag>
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

        <label className="kb-search">
          <span>⌕</span>
          <input
            value={search}
            onChange={event => setSearch(event.target.value)}
            placeholder={lang === 'it' ? 'Cerca in tutti i contenuti…' : 'Search all content…'}
          />
          {search && <button onClick={() => setSearch('')} aria-label="Clear search">×</button>}
        </label>

        <div className="kb-nav">
          {groups.length === 0 && (
            <p className="kb-empty">
              {lang === 'it' ? 'Nessun contenuto trovato.' : 'No matching content.'}
            </p>
          )}

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
                  <span className="kb-entry-copy">
                    <strong>{entry.title}</strong>
                    {search && entry.excerpt && <small>{entry.excerpt}</small>}
                  </span>
                </button>
              ))}
            </div>
          ))}
        </div>
      </aside>

      <article className="kb-viewer">
        <div className="kb-toolbar">
          <div className="kb-toolbar-path">
            <span>{kind === 'markdown' ? 'MARKDOWN' : 'SOURCE'}</span>
            <code>{selectedPath}</code>
          </div>

          <div className="kb-toolbar-actions">
            <button onClick={copyLink}>{copied ? 'Copied ✓' : (lang === 'it' ? 'Copia link' : 'Copy link')}</button>
            <a
              href={`https://github.com/KeyserDSoze/Agent365/blob/main/${selectedPath}`}
              target="_blank"
              rel="noreferrer"
            >
              GitHub ↗
            </a>
          </div>
        </div>

        <div className="markdown-body">
          {loading && <p className="kb-state">{lang === 'it' ? 'Caricamento…' : 'Loading…'}</p>}
          {error && <p className="kb-error">{error}</p>}
          {!loading && !error && (
            <ReactMarkdown
              remarkPlugins={[remarkGfm]}
              components={{
                h1: heading('h1'),
                h2: heading('h2'),
                h3: heading('h3'),
                h4: heading('h4'),
                a({ href, children, ...props }) {
                  if (href?.startsWith('#')) {
                    return (
                      <a
                        href={href}
                        onClick={event => {
                          event.preventDefault()
                          scrollToHeading(href)
                        }}
                        {...props}
                      >
                        {children}
                      </a>
                    )
                  }

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

          {!loading && !error && (
            <nav className="kb-page-nav">
              <button disabled={!previous} onClick={() => navigate(previous?.path)}>
                <span>←</span>
                <small>{previous?.title || (lang === 'it' ? 'Inizio' : 'Start')}</small>
              </button>
              <button disabled={!next} onClick={() => navigate(next?.path)}>
                <small>{next?.title || (lang === 'it' ? 'Fine' : 'End')}</small>
                <span>→</span>
              </button>
            </nav>
          )}
        </div>
      </article>
    </div>
  )
}
