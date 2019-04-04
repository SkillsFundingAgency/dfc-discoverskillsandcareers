const helpers = require('./modules/helpers.js')
const analytics = require('./modules/analytics.js')
const results = require('./modules/results.js')

analytics.init()

if (helpers.isPage('app-page--results')) {
  results.short()
}

if (helpers.isPage('app-page--results-long')) {
  results.long()
}
