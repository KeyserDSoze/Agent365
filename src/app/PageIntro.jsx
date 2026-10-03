export default function PageIntro({ kicker, title, text, actions }) {
  return (
    <div className="page-intro">
      <p className="kicker">{kicker}</p>
      <h1>{title}</h1>
      <p>{text}</p>
      {actions && <div className="page-intro-actions">{actions}</div>}
    </div>
  )
}
