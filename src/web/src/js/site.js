const analytics = require('./modules/analytics.js')
const results = require('./modules/results.js')

const isPage = className => document.getElementsByClassName(className).length

analytics.init()

if (isPage('app-page--results')) {
  results.init()
}
