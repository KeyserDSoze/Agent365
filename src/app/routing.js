export const navRoutes = [
  ['overview', '/'],
  ['architecture', '/architecture'],
  ['domains', '/capabilities'],
  ['examples', '/assets'],
  ['training', '/academy'],
  ['labs', '/labs'],
  ['developer', '/developer'],
  ['operations', '/operations'],
  ['knowledge', '/knowledge'],
  ['sources', '/sources']
]

export function knowledgeRoute(path) {
  return '/knowledge/' + path
    .split('/')
    .map(segment => encodeURIComponent(segment))
    .join('/')
}

export function pathFromKnowledgeWildcard(wildcard = '') {
  return wildcard
    .split('/')
    .filter(Boolean)
    .map(segment => decodeURIComponent(segment))
    .join('/')
}
