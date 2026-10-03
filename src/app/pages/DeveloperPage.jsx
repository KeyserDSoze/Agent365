import { Link } from 'react-router-dom'
import { copy } from '../content.js'
import { knowledgeRoute } from '../routing.js'
import PageIntro from '../PageIntro.jsx'
import JourneyNext from '../JourneyNext.jsx'

export default function DeveloperPage({ lang }) {
  const t = copy[lang]
  return (
    <div className="route-page">
      <section className="section developer">
        <PageIntro kicker="07 · DEVELOPER PATH" title={t.developerTitle} text={t.developerIntro} />
        <div className="developer-flow">
          {t.developerSteps.map(([code, title, text, cta, path]) => (
            <Link className="dev-step" to={knowledgeRoute(path)} key={code}>
              <span className="dev-code">{code}</span>
              <div>
                <h3>{title}</h3>
                <p>{text}</p>
                <strong>{cta} →</strong>
              </div>
            </Link>
          ))}
        </div>
        <div className="developer-terminal">
          <div className="window-dots"><i/><i/><i/></div>
          <code>gh skill add microsoft/agent365-skills</code>
          <span>→ setup → register → observability → tools/DLP → validate → operate</span>
        </div>

        <JourneyNext
          lang={lang}
          stageId="build"
          title={lang === 'it'
            ? 'Quando il runtime funziona, il passo successivo è dimostrare che identità, dati e tool sono governabili.'
            : 'Once the runtime works, the next step is proving that identity, data and tools are governable.'}
        />
      </section>
    </div>
  )
}
