import { useEffect, useMemo, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import ReactMarkdown from 'react-markdown'
import remarkGfm from 'remark-gfm'
import { knowledgeRoute, pathFromKnowledgeWildcard } from './routing.js'

function resolveRelative(currentPath, href) {
  if (!href || href.startsWith('#') || /^https?:\/\//i.test(href)) return null
  const base = new URL(currentPath, 'https://knowledge.local/')
  const resolved = new URL(href, base)
  return decodeURIComponent(resolved.pathname.replace(/^\//, ''))
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

function stripPrimaryHeading(markdown, title) {
  const first = markdown.match(/^#\s+(.+)\r?\n+/)
  if (!first || first[1].trim() !== title?.trim()) return markdown
  return markdown.slice(first[0].length)
}

export default function MarkdownViewer({ lang }) {
  const params = useParams()
  const navigate = useNavigate()
  const selectedPath = pathFromKnowledgeWildcard(params['*'] || '')
  const [index, setIndex] = useState([])
  const [body, setBody] = useState('')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [copied, setCopied] = useState(false)
  const baseUrl = import.meta.env.BASE_URL + 'content/'

  useEffect(() => {
    fetch(baseUrl + 'index.json')
      .then(response => {
        if (!response.ok) throw new Error('HTTP ' + response.status)
        return response.json()
      })
      .then(data => setIndex(data.entries || []))
      .catch(err => setError('Knowledge index unavailable: ' + err.message))
  }, [baseUrl])

  useEffect(() => {
    if (!selectedPath) return
    setLoading(true)
    setError('')
    setCopied(false)

    const localPath = selectedPath.split('/').map(encodeURIComponent).join('/')
    fetch(baseUrl + localPath)
      .then(response => {
        if (!response.ok) throw new Error('HTTP ' + response.status)
        return response.text()
      })
      .then(setBody)
      .catch(err => {
        setBody('')
        setError('Unable to load ' + selectedPath + ': ' + err.message)
      })
      .finally(() => setLoading(false))
  }, [baseUrl, selectedPath])

  const entry = index.find(item => item.path === selectedPath)
  const kind = entry?.kind || (selectedPath.endsWith('.md') ? 'markdown' : 'code')
  const title = entry?.title || selectedPath.split('/').pop() || 'Document'
  const fence = String.fromCharCode(96).repeat(3)
  const markdown = kind === 'markdown'
    ? stripPrimaryHeading(body, title)
    : fence + codeLanguage(selectedPath) + '\n' + body + '\n' + fence

  const toc = useMemo(() => {
    if (kind !== 'markdown') return []
    return body
      .split(/\r?\n/)
      .map(line => line.match(/^(##|###)\s+(.+)$/))
      .filter(Boolean)
      .map(match => {
        const label = match[2]
          .replace(/\[(.*?)\]\(.*?\)/g, '$1')
          .replace(/[*_\x60]/g, '')
        return { level: match[1].length, label, id: slugify(label) }
      })
  }, [body, kind])

  const languageEntries = useMemo(
    () => index.filter(item => item.language === 'shared' || item.language === lang),
    [index, lang]
  )
  const currentIndex = languageEntries.findIndex(item => item.path === selectedPath)
  const previous = currentIndex > 0 ? languageEntries[currentIndex - 1] : null
  const next = currentIndex >= 0 && currentIndex < languageEntries.length - 1
    ? languageEntries[currentIndex + 1]
    : null

  useEffect(() => {
    if (!selectedPath && index.length) {
      navigate(
        knowledgeRoute(lang === 'it' ? 'docs/README.md' : 'docs/en/README.md'),
        { replace: true }
      )
    }
  }, [selectedPath, index, lang, navigate])

  useEffect(() => {
    if (!loading && window.location.hash) {
      const id = decodeURIComponent(window.location.hash.slice(1))
      requestAnimationFrame(() => document.getElementById(id)?.scrollIntoView({ block: 'start' }))
    }
  }, [loading, body])

  const copyLink = async () => {
    try {
      await navigator.clipboard.writeText(window.location.href)
      setCopied(true)
      window.setTimeout(() => setCopied(false), 1600)
    } catch {
      setCopied(false)
    }
  }

  const downloadLocal = () => {
    const blob = new Blob([body], {
      type: kind === 'markdown' ? 'text/markdown;charset=utf-8' : 'text/plain;charset=utf-8'
    })
    const url = URL.createObjectURL(blob)
    const anchor = document.createElement('a')
    anchor.href = url
    anchor.download = selectedPath.split('/').pop() || 'document.md'
    document.body.appendChild(anchor)
    anchor.click()
    anchor.remove()
    URL.revokeObjectURL(url)
  }

  const heading = Tag => ({ children, ...props }) => {
    const id = slugify(headingText(children))
    return <Tag id={id} {...props}>{children}</Tag>
  }

  const githubUrl = 'https://github.com/KeyserDSoze/Agent365/blob/main/' + selectedPath

  if (!selectedPath) {
    return (
      <div className="route-page">
        <section className="section"><p className="kb-state">Loading…</p></section>
      </div>
    )
  }

  return (
    <div className="route-page document-route">
      <div className="doc-breadcrumb">
        <Link to="/knowledge">Knowledge base</Link>
        <span>/</span>
        <code>{selectedPath}</code>
      </div>

      <header className="doc-hero">
        <div>
          <span className="doc-kind">
            {kind === 'markdown' ? 'MARKDOWN · LOCAL BUILD COPY' : 'SOURCE · LOCAL BUILD COPY'}
          </span>
          <h1>{title}</h1>
          <p>{entry?.excerpt || selectedPath}</p>
        </div>
        <div className="doc-actions">
          <button onClick={copyLink}>{copied ? 'Copied ✓' : (lang === 'it' ? 'Copia link' : 'Copy link')}</button>
          <button onClick={downloadLocal}>{lang === 'it' ? 'Scarica file' : 'Download file'} ↓</button>
          <a href={githubUrl} target="_blank" rel="noreferrer">
            {lang === 'it' ? 'Apri su GitHub' : 'Open on GitHub'} ↗
          </a>
        </div>
      </header>

      <div className="doc-layout">
        <aside className="doc-toc">
          <div className="doc-toc-sticky">
            <span>{lang === 'it' ? 'IN QUESTA PAGINA' : 'ON THIS PAGE'}</span>
            {toc.length ? (
              <nav>
                {toc.map(item => (
                  <a
                    href={'#' + item.id}
                    className={item.level === 3 ? 'nested' : ''}
                    key={item.level + '-' + item.id}
                  >
                    {item.label}
                  </a>
                ))}
              </nav>
            ) : (
              <small>{lang === 'it' ? 'Documento senza sezioni.' : 'No section headings.'}</small>
            )}
            <div className="doc-source-note">
              <strong>Source of truth</strong>
              <p>{lang === 'it'
                ? 'Questa pagina usa la copia sincronizzata nella build. Il file originale resta nella repository.'
                : 'This page uses the build-synchronized copy. The original file remains in the repository.'}</p>
            </div>
          </div>
        </aside>

        <article className="doc-page">
          {loading && <p className="kb-state">{lang === 'it' ? 'Caricamento…' : 'Loading…'}</p>}
          {error && <p className="kb-error">{error}</p>}
          {!loading && !error && (
            <div className="markdown-body">
              <ReactMarkdown
                remarkPlugins={[remarkGfm]}
                components={{
                  h1: heading('h1'),
                  h2: heading('h2'),
                  h3: heading('h3'),
                  h4: heading('h4'),
                  a({ href, children, ...props }) {
                    if (href?.startsWith('#')) {
                      return <a href={href} {...props}>{children}</a>
                    }

                    const internal = resolveRelative(selectedPath, href)
                    const knowledgeFile = internal && index.some(item => item.path === internal)

                    if (knowledgeFile) {
                      return <Link to={knowledgeRoute(internal)} {...props}>{children}</Link>
                    }

                    return <a href={href} target="_blank" rel="noreferrer" {...props}>{children}</a>
                  }
                }}
              >
                {markdown}
              </ReactMarkdown>

              <nav className="kb-page-nav">
                {previous ? (
                  <Link to={knowledgeRoute(previous.path)}>
                    <span>←</span><small>{previous.title}</small>
                  </Link>
                ) : <span />}
                {next ? (
                  <Link to={knowledgeRoute(next.path)}>
                    <small>{next.title}</small><span>→</span>
                  </Link>
                ) : <span />}
              </nav>
            </div>
          )}
        </article>
      </div>
    </div>
  )
}
