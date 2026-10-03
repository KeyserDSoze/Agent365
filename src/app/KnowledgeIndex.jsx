import { useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import { knowledgeRoute } from './routing.js'

function groupLabel(path, lang) {
  if (path === 'README.md') return 'Repository'
  if (path.includes('/tutorials/')) return lang === 'it' ? 'Tutorial pratici' : 'Hands-on tutorials'
  if (path.startsWith('docs/en/')) return 'Academy EN'
  if (path.startsWith('docs/')) return 'Academy IT'
  if (path.startsWith('examples/')) return lang === 'it' ? 'Asset & esempi' : 'Assets & examples'
  if (path.startsWith('samples/')) return lang === 'it' ? 'Sample tecnici' : 'Technical samples'
  return lang === 'it' ? 'Altro' : 'Other'
}

export default function KnowledgeIndex({ lang }) {
  const [entries, setEntries] = useState([])
  const [query, setQuery] = useState('')
  const [error, setError] = useState('')
  const baseUrl = import.meta.env.BASE_URL + 'content/'

  useEffect(() => {
    fetch(baseUrl + 'index.json')
      .then(response => {
        if (!response.ok) throw new Error('HTTP ' + response.status)
        return response.json()
      })
      .then(data => setEntries(data.entries || []))
      .catch(err => setError(err.message))
  }, [baseUrl])

  const results = useMemo(() => {
    const languageEntries = entries.filter(entry => entry.language === 'shared' || entry.language === lang)
    const q = query.trim().toLowerCase()
    if (!q) return languageEntries

    const terms = q.split(/\s+/).filter(Boolean)
    return languageEntries
      .map(entry => {
        const haystack = entry.search || (entry.title + ' ' + entry.path + ' ' + (entry.excerpt || '')).toLowerCase()
        const score = terms.reduce((acc, term) => {
          if (!haystack.includes(term)) return -999
          if (entry.title.toLowerCase().includes(term)) return acc + 6
          if (entry.path.toLowerCase().includes(term)) return acc + 3
          return acc + 1
        }, 0)
        return { entry, score }
      })
      .filter(item => item.score >= 0)
      .sort((a, b) => b.score - a.score || a.entry.path.localeCompare(b.entry.path))
      .map(item => item.entry)
  }, [entries, lang, query])

  const groups = useMemo(() => {
    const grouped = new Map()
    for (const entry of results) {
      const label = groupLabel(entry.path, lang)
      if (!grouped.has(label)) grouped.set(label, [])
      grouped.get(label).push(entry)
    }
    return [...grouped.entries()]
  }, [results, lang])

  return (
    <div className="route-page knowledge-index-page">
      <section className="section">
        <div className="knowledge-index-hero">
          <div>
            <p className="kicker">09 · KNOWLEDGE BASE</p>
            <h1>{lang === 'it' ? 'La repository, resa leggibile.' : 'The repository, made readable.'}</h1>
            <p>{lang === 'it'
              ? 'Cerca manuali, tutorial, sample e asset. Ogni risultato apre una pagina HTML dedicata, con URL condivisibile e link al Markdown originale.'
              : 'Search manuals, tutorials, samples and assets. Every result opens a dedicated HTML page with a shareable URL and a link to the original Markdown.'}</p>
          </div>

          <label className="knowledge-search">
            <span>⌕</span>
            <input
              value={query}
              onChange={event => setQuery(event.target.value)}
              placeholder={lang === 'it' ? 'Cerca nella knowledge base…' : 'Search the knowledge base…'}
            />
            {query && <button onClick={() => setQuery('')} aria-label="Clear search">×</button>}
          </label>
        </div>

        {error && <p className="kb-error">Knowledge index unavailable: {error}</p>}

        <div className="knowledge-groups">
          {groups.map(([label, items]) => (
            <section className="knowledge-group" key={label}>
              <div className="knowledge-group-head">
                <h2>{label}</h2>
                <span>{items.length}</span>
              </div>
              <div className="knowledge-card-grid">
                {items.map(entry => (
                  <Link to={knowledgeRoute(entry.path)} className="knowledge-card" key={entry.path}>
                    <div className="knowledge-card-meta">
                      <span>{entry.kind === 'markdown' ? 'ARTICLE' : 'SOURCE'}</span>
                      <code>{entry.path}</code>
                    </div>
                    <h3>{entry.title}</h3>
                    <p>{entry.excerpt}</p>
                    <strong>{lang === 'it' ? 'Apri pagina' : 'Open page'} →</strong>
                  </Link>
                ))}
              </div>
            </section>
          ))}
        </div>

        {!error && entries.length > 0 && results.length === 0 && (
          <p className="knowledge-empty">{lang === 'it' ? 'Nessun contenuto trovato.' : 'No content found.'}</p>
        )}
      </section>
    </div>
  )
}
