import { Link, useLocation } from 'react-router-dom'
import { getJourneyStages, getStageForLocation } from './journey.js'

export default function JourneyRibbon({ lang }) {
  const location = useLocation()
  const stages = getJourneyStages(lang)
  const current = getStageForLocation(location.pathname)

  return (
    <div className="journey-ribbon" aria-label={lang === 'it' ? 'Percorso Agent 365' : 'Agent 365 journey'}>
      <Link className="journey-ribbon-label" to="/journey">
        <span>{lang === 'it' ? 'PERCORSO' : 'JOURNEY'}</span>
        <strong>{lang === 'it' ? 'Dalla teoria alla delivery' : 'From theory to delivery'}</strong>
      </Link>
      <div className="journey-ribbon-stages">
        {stages.map(stage => (
          <Link
            to={stage.route}
            key={stage.id}
            className={current === stage.id ? 'active' : ''}
            title={stage.title}
          >
            <small>{stage.number}</small>
            <span>{stage.label}</span>
          </Link>
        ))}
      </div>
    </div>
  )
}
