import { Link } from 'react-router-dom'
import { getJourneyStages } from '../journey.js'
import { knowledgeRoute } from '../routing.js'
import PageIntro from '../PageIntro.jsx'

function humanize(path) {
  return (path.split('/').pop() || path)
    .replace(/\.(md|kql|csv|json)$/i, '')
    .replace(/^\d+-/, '')
    .replaceAll('-', ' ')
    .replace(/\b\w/g, value => value.toUpperCase())
}

export default function JourneyPage({ lang }) {
  const stages = getJourneyStages(lang)

  const entryPoints = lang === 'it'
    ? [
        ['Devo capire Agent 365', 'Parti dal mental model e dall’architettura.', 'understand'],
        ['Devo fare un assessment', 'Parti da readiness, identity e customer discovery.', 'prepare'],
        ['Devo costruire un POC', 'Vai al developer path e ai sample eseguibili.', 'build'],
        ['Devo mettere governance', 'Vai a Defender, Purview, KQL e tool governance.', 'govern'],
        ['Devo portarlo in esercizio', 'Vai a observability, reliability e incident response.', 'operate']
      ]
    : [
        ['I need to understand Agent 365', 'Start from the mental model and architecture.', 'understand'],
        ['I need to run an assessment', 'Start from readiness, identity and customer discovery.', 'prepare'],
        ['I need to build a POC', 'Go to the developer path and runnable samples.', 'build'],
        ['I need governance', 'Go to Defender, Purview, KQL and tool governance.', 'govern'],
        ['I need to operate it', 'Go to observability, reliability and incident response.', 'operate']
      ]

  return (
    <div className="route-page">
      <section className="section journey-page">
        <PageIntro
          kicker={lang === 'it' ? 'PERCORSO GUIDATO' : 'GUIDED JOURNEY'}
          title={lang === 'it' ? 'Un solo filo: dalla comprensione alla delivery.' : 'One thread: from understanding to delivery.'}
          text={lang === 'it'
            ? 'Il sito non è una raccolta di pagine: è un percorso. Puoi seguirlo in sequenza oppure entrare direttamente dalla necessità che hai oggi.'
            : 'The site is not a collection of pages: it is a journey. Follow it in sequence or jump directly to the need you have today.'}
        />

        <div className="journey-entry-grid">
          {entryPoints.map(([title, text, stageId]) => {
            const stage = stages.find(item => item.id === stageId)
            return (
              <Link to={stage.route} className="journey-entry" key={stageId}>
                <span>{stage.number}</span>
                <div><strong>{title}</strong><p>{text}</p></div>
                <b>→</b>
              </Link>
            )
          })}
        </div>

        <div className="journey-stage-list">
          {stages.map((stage, index) => (
            <article className="journey-stage-card" id={stage.id} key={stage.id}>
              <div className="journey-stage-number">{stage.number}</div>
              <div className="journey-stage-copy">
                <span>{stage.label}</span>
                <h2>{stage.title}</h2>
                <p>{stage.summary}</p>
                <div className="journey-outcome">
                  <small>{lang === 'it' ? 'RISULTATO' : 'OUTCOME'}</small>
                  <strong>{stage.outcome}</strong>
                </div>
              </div>
              <div className="journey-stage-links">
                <Link className="primary link-button" to={stage.route}>
                  {lang === 'it' ? 'Apri la tappa' : 'Open stage'} →
                </Link>
                <div>
                  <span>{lang === 'it' ? 'Letture consigliate' : 'Recommended reading'}</span>
                  {stage.docs.slice(0, 4).map(path => (
                    <Link to={knowledgeRoute(path)} key={path}>
                      {humanize(path)} →
                    </Link>
                  ))}
                </div>
              </div>
              {index < stages.length - 1 && <i className="journey-stage-connector">↓</i>}
            </article>
          ))}
        </div>
      </section>
    </div>
  )
}
