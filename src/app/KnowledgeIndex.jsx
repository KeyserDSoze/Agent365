import { useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import { getJourneyStages, getStageForDocument } from './journey.js'
import { knowledgeRoute } from './routing.js'

function fallbackGroup(path, lang) {
  if (path === 'README.md') return lang === 'it' ? 'Repository & orientamento' : 'Repository & orientation'
  if (path.includes('training') || path.includes('labs')) return lang === 'it' ? 'Percorsi trasversali' : 'Cross-cutting paths'
  if (path.includes('sources')) return lang === 'it' ? 'Fonti e riferimenti' : 'Sources & references'
  return lang === 'it' ? 'Reference & altri contenuti' : 'Reference & other content'
}

export default function KnowledgeIndex({ lang }) {
  const [entries, setEntries] = useState([])
  const [query, setQuery] = useState('')
  const [error, setError] = useState('')
  const baseUrl = import.meta.env.BASE_URL + 'content/'
  const stages = getJourneyStages(lang)

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
        const stageId = getStageForDocument(entry.path)
        const stage = stages.find(item => item.id === stageId)
        const stageText = stage ? stage.title + ' ' + stage.summary : ''
        const haystack = (entry.search || (entry.title + ' ' + entry.path + ' ' + (entry.excerpt || ''))) + ' ' + stageText.toLowerCase()

        const score = terms.reduce((acc, term) => {
          if (!haystack.includes(term)) return -999
          if (entry.title.toLowerCase().includes(term)) return acc + 6
          if (entry.path.toLowerCase().includes(term)) return acc + 3
          if (stageText.toLowerCase().includes(term)) return acc + 2
          return acc + 1
        }, 0)

        return { entry, score }
      })
      .filter(item => item.score >= 0)
      .sort((a, b) => b.score - a.score || a.entry.path.localeCompare(b.entry.path))
      .map(item => item.entry)
  }, [entries, lang, query, stages])

  const groups = useMemo(() => {
    const grouped = new Map()

    for (const entry of results) {
      const stageId = getStageForDocument(entry.path)
      const stage = stages.find(item => item.id === stageId)
      const key = stage ? stage.id : fallbackGroup(entry.path, lang)

      if (!grouped.has(key)) {
        grouped.set(key, {
          stage,
          label: stage ? stage.number + ' · ' + stage.title : key,
          summary: stage ? stage.summary : '',
          items: []
        })
      }

      grouped.get(key).items.push(entry)
    }

    const ordered = []
    for (const stage of stages) {
      if (grouped.has(stage.id)) ordered.push(grouped.get(stage.id))
    }

    for (const [key, value] of grouped) {
      if (!stages.some(stage => stage.id === key)) ordered.push(value)
    }

    return ordered
  }, [results, stages, lang])

  return (
    <div className="route-page knowledge-index-page">
      <section className="section">
        <div className="knowledge-index-hero">
          <div>
            <p className="kicker">KNOWLEDGE BASE</p>
            <h1>{lang === 'it' ? 'Cerca per lavoro da fare, non per cartella.' : 'Search by job to do, not by folder.'}</h1>
            <p>{lang === 'it'
              ? 'Manuali, tutorial, sample e asset sono raggruppati secondo il percorso Capire → Preparare → Costruire → Governare → Operare. Il path della repository resta visibile, ma non guida più l’esperienza.'
              : 'Manuals, tutorials, samples and assets are grouped by Understand → Prepare → Build → Govern → Operate. Repository paths remain visible, but no longer drive the experience.'}</p>
            <Link className="knowledge-journey-link" to="/journey">
              {lang === 'it' ? 'Non sai dove iniziare? Apri il percorso guidato' : 'Not sure where to start? Open the guided journey'} →
            </Link>
          </div>

          <label className="knowledge-search">
            <span>⌕</span>
            <input
              value={query}
              onChange={event => setQuery(event.target.value)}
              placeholder={lang === 'it' ? 'Cerca un problema, capability o output…' : 'Search a problem, capability or output…'}
            />
            {query && <button onClick={() => setQuery('')} aria-label="Clear search">×</button>}
          </label>
        </div>

        {error && <p className="kb-error">Knowledge index unavailable: {error}</p>}

        <div className="knowledge-groups">
          {groups.map(group => (
            <section className="knowledge-group" key={group.label}>
              <div className="knowledge-group-head knowledge-group-head-rich">
                <div>
                  <h2>{group.label}</h2>
                  {group.summary && <p>{group.summary}</p>}
                </div>
                <span>{group.items.length}</span>
              </div>

              <div className="knowledge-card-grid">
                {group.items.map(entry => {
                  const stageId = getStageForDocument(entry.path)
                  const stage = stages.find(item => item.id === stageId)

                  return (
                    <Link to={knowledgeRoute(entry.path)} className="knowledge-card" key={entry.path}>
                      <div className="knowledge-card-meta">
                        <span>{stage ? stage.label.toUpperCase() : (entry.kind === 'markdown' ? 'ARTICLE' : 'SOURCE')}</span>
                        <code>{entry.path}</code>
                      </div>
                      <h3>{entry.title}</h3>
                      <p>{entry.excerpt}</p>
                      <strong>{lang === 'it' ? 'Apri nel percorso' : 'Open in journey'} →</strong>
                    </Link>
                  )
                })}
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
