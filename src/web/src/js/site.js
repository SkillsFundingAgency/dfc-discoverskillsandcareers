const analytics = require('./analytics.js')
const results = require('./results.js')

const isPage = className => document.getElementsByClassName(className).length

analytics.init()

if (isPage('app-page--results')) {
  results.init()
}
