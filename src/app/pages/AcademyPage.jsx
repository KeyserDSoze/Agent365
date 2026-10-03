import { Link } from 'react-router-dom'
import { copy } from '../content.js'
import { knowledgeRoute } from '../routing.js'
import PageIntro from '../PageIntro.jsx'

export default function AcademyPage({ lang }) {
  const t = copy[lang]
  const stageLabels = lang === 'it'
    ? [
        ['01 · CAPIRE', '02 · PREPARARE'],
        ['04 · GOVERNARE'],
        ['03 · COSTRUIRE', '05 · OPERARE']
      ]
    : [
        ['01 · UNDERSTAND', '02 · PREPARE'],
        ['04 · GOVERN'],
        ['03 · BUILD', '05 · OPERATE']
      ]

  return (
    <div className="route-page">
      <section className="section training">
        <PageIntro
          kicker="ACADEMY"
          title={t.trainingTitle}
          text={lang === 'it'
            ? 'L’Academy attraversa lo stesso percorso del sito. Le settimane raggruppano le tappe per apprendimento, ma gli output prodotti confluiscono negli stessi assessment, POC, governance pack e runbook.'
            : 'The Academy follows the same journey as the site. Weeks group stages for learning, while the produced outputs feed the same assessments, POCs, governance packs and runbooks.'}
        />

        <div className="weeks">
          {t.weeks.map((week, index) => (
            <article className="week" key={week.title}>
              <div className="week-stage-tags">
                {stageLabels[index].map(label => <span key={label}>{label}</span>)}
              </div>
              <div className="week-head"><span>0{index + 1}</span><h3>{week.title}</h3></div>
              <ol>{week.days.map(day => <li key={day}>{day}</li>)}</ol>
            </article>
          ))}
        </div>

        <div className="route-cta academy-actions">
          <Link className="primary link-button" to={knowledgeRoute(lang === 'it' ? 'docs/README.md' : 'docs/en/README.md')}>
            {lang === 'it' ? 'Apri l’Academy completa' : 'Open the full Academy'} →
          </Link>
          <Link className="secondary link-button" to="/labs">
            {lang === 'it' ? 'Vai ai lab eseguibili' : 'Open executable labs'} →
          </Link>
          <Link className="secondary link-button" to="/journey">
            {lang === 'it' ? 'Rivedi il percorso end-to-end' : 'Review the end-to-end journey'} →
          </Link>
        </div>
      </section>
    </div>
  )
}
