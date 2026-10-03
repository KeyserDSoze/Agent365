import { Link } from 'react-router-dom'
import PageIntro from '../PageIntro.jsx'

export default function NotFoundPage({ lang }) {
  return (
    <div className="route-page">
      <section className="section not-found">
        <PageIntro
          kicker="404"
          title={lang === 'it' ? 'Pagina non trovata.' : 'Page not found.'}
          text={lang === 'it'
            ? 'La route non esiste oppure il contenuto è stato spostato.'
            : 'This route does not exist or the content has moved.'}
          actions={<Link className="primary link-button" to="/">{lang === 'it' ? 'Torna alla home' : 'Back home'}</Link>}
        />
      </section>
    </div>
  )
}
