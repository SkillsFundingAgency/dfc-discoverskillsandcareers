const helpers = require('./modules/helpers.js')
const analytics = require('./modules/analytics.js')
const results = require('./modules/results.js')
const breadcrumbs = require('./modules/breadcrumbs.js')

analytics.init()
breadcrumbs.init()

if (helpers.isPage('app-page--results')) {
  results.short()
}

if (helpers.isPage('app-page--results-long')) {
  results.long()
}
