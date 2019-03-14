const analytics = require('./modules/analytics.js')
const results = require('./modules/results.js')

const isPage = className => document.getElementsByClassName(className).length

analytics.init()

if (isPage('app-page--start')) {
  analytics.startSurvey()
}

if (isPage('app-page--questions')) {
  analytics.updateSurvey()
}

if (isPage('app-page--results')) {
  results.init()
  analytics.completeSurvey()
}
