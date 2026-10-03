import { Link } from 'react-router-dom'
import { getJourneyStage, getJourneyStages } from './journey.js'

export default function JourneyNext({ lang, stageId, title, children }) {
  const stages = getJourneyStages(lang)
  const index = stages.findIndex(stage => stage.id === stageId)
  const current = getJourneyStage(stageId, lang)
  const next = index >= 0 && index < stages.length - 1 ? stages[index + 1] : null

  if (!current) return null

  return (
    <section className="journey-next">
      <div className="journey-next-current">
        <span>{lang === 'it' ? 'DOVE SEI' : 'YOU ARE HERE'}</span>
        <strong>{current.number} · {current.title}</strong>
        <p>{title || current.outcome}</p>
        {children}
      </div>
      <div className="journey-next-action">
        {next ? (
          <>
            <span>{lang === 'it' ? 'PROSSIMA TAPPA' : 'NEXT STAGE'}</span>
            <strong>{next.number} · {next.title}</strong>
            <p>{next.summary}</p>
            <Link to={next.route}>
              {lang === 'it' ? 'Continua il percorso' : 'Continue the journey'} →
            </Link>
          </>
        ) : (
          <>
            <span>{lang === 'it' ? 'PERCORSO COMPLETATO' : 'JOURNEY COMPLETE'}</span>
            <strong>{lang === 'it' ? 'Dalla conoscenza alla delivery' : 'From knowledge to delivery'}</strong>
            <p>{lang === 'it'
              ? 'Ora puoi tornare al percorso completo, aprire il customer delivery pack o usare la knowledge base come riferimento.'
              : 'You can now return to the full journey, open the customer delivery pack or use the knowledge base as reference.'}</p>
            <Link to="/journey">{lang === 'it' ? 'Rivedi il percorso' : 'Review the journey'} →</Link>
          </>
        )}
      </div>
    </section>
  )
}
