/* eslint-disable no-console */
"use strict";

// helpers 

function handlePa11yError () {
    process.exit(1);
} 

function handlebrowserStackError () {
    process.exit(1);
}

function handleLighthouseError () {
    process.exit(1);
}

// requires

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    sass = require("gulp-sass"),
    eslint = require("gulp-eslint"),
    nunjucks = require('gulp-nunjucks-render'),
    connect = require('gulp-connect'),
    mocha = require('gulp-mocha'),
    sassLint = require('gulp-sass-lint'),
    {protractor} = require('gulp-protractor'),
    header = require('gulp-header'),
    filter = require('gulp-filter'),
    rev = require('gulp-rev'),
    revRewrite = require('gulp-rev-rewrite'),
    replace = require('gulp-replace'),
    merge = require('merge-stream'),
    babel = require("gulp-babel"),
    autoprefixer = require('gulp-autoprefixer'),
    standard = require('gulp-standard');

// paths

var paths = {
    src: "src/",
    dist: "dist/",
    temp: ".temp/",
    tests: "tests/"
};

// paths – input

paths.html = paths.src + "templates/**/*.html";
paths.nunjucks = paths.src + "partials/**/*.njk";
paths.scss = paths.src + "scss/**/*.scss";
paths.images = paths.src + "images/**/*";
paths.js = paths.src + "js/**/*.js";

// paths - output

paths.css = paths.temp + "css/**/*.css";
paths.minCss = paths.temp + "css/**/*.min.css";
paths.concatMinCssDest = paths.dist + "css/site.min.css";
paths.minJs = paths.temp + "js/**/*.min.js";
paths.concatJsDest = paths.dist + "js/site.js";
paths.concatMinJsDest = paths.dist + "js/site.min.js";
paths.assetsDest = paths.dist + "assets/";
paths.cssDest = paths.assetsDest + "css/";
paths.jsDest = paths.assetsDest + "js/";
paths.imagesDest = paths.assetsDest + "images/";

// paths - tests

paths.accessibilty = paths.tests + "accessibility.spec.js";
paths.performance = paths.tests + "performance.spec.js";
paths.browserStackConf = paths.tests + "conf/conf.js";
paths.browserStackSpec = paths.tests + "specs/*.spec.js";

const testServerOptions = {
    port: 3000,
    root: paths.dist,
}

// tasks

gulp.task('assets', function () {
    var govuk = gulp.src(['./node_modules/govuk-frontend/assets/**/*']).pipe(gulp.dest(paths.assetsDest))
    var images = gulp.src(paths.images).pipe(gulp.dest(paths.imagesDest))
    return merge(govuk, images);
});

gulp.task("clean:js", function (cb) {
    rimraf(paths.jsDest + "", cb);
    rimraf(paths.concatMinJsDest + "", cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.css, cb);
    rimraf(paths.concatMinCssDest, cb);
});

gulp.task("clean:assets", function (cb) {
    rimraf(paths.assetsDest, cb);
});

gulp.task("sass", function () {
    return gulp.src(paths.scss)
        .pipe(sassLint())
        .pipe(sassLint.format())
        .pipe(sassLint.failOnError())
        .pipe(sass({
            includePaths: 'node_modules'
        }))
        .pipe(autoprefixer({
            browsers: ['last 3 versions']
        }))
        .pipe(gulp.dest(paths.cssDest))
        .pipe(connect.reload());
});

gulp.task("js", function () {
    return gulp.src(paths.js)
        .pipe(standard())
        .pipe(standard.reporter('default'))
        .pipe(babel())
        .pipe(gulp.dest(paths.jsDest))
        .pipe(connect.reload());
});

gulp.task("min:js", function () {
    return gulp.src(paths.jsDest + 'site.js')
        .pipe(uglify())
        .pipe(gulp.dest(paths.jsDest))
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatMinCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("eslint", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(eslint())
        .pipe(eslint.format())
        .pipe(eslint.failAfterError());
});

gulp.task('html', function() {
    return gulp.src(paths.html)
        .pipe(nunjucks({
          path: [
            "node_modules/govuk-frontend/", 
            "node_modules/govuk-frontend/components/", 
            "src/templates/",
            "src/partials/"]
        }))
        .pipe(gulp.dest(paths.dist))
        .pipe(connect.reload());
});

gulp.task('rev', () => {
    const assetFilter = filter(['**/*', '!**/*.html', '!**/*.woff*', '!**/*.eot'], { restore: true });
  
    return gulp.src(paths.dist + '**/*')
      .pipe(assetFilter)
      .pipe(rev()) // Rename all files except index.html
      .pipe(assetFilter.restore)
      .pipe(revRewrite()) // Substitute in new filenames
      .pipe(gulp.dest(paths.dist));
});

gulp.task('headers', () => {
    return gulp.src(paths.dist + '**/*.html')
        .pipe(header('\ufeff'))
        .pipe(gulp.dest(paths.dist));
});
  
gulp.task('connect', function() {
  connect.server({
    root: paths.dist,
    port: 3000,
    livereload: true
  });
});

// QA

gulp.task('pa11y', function() {
    return gulp.src(paths.accessibilty, {read: false})
        .pipe(mocha({exit: true}))
        .once("error", handlePa11yError)
});

gulp.task('browserStack', function(done) {
    gulp.src([paths.browserStackSpec])
        .pipe(protractor({
            configFile: paths.browserStackConf
        }))
        .on("error", handlebrowserStackError)
        .on("end", done);
});

gulp.task('startTestServer', function(done) {
    connect.server(testServerOptions);
    done();
});

gulp.task('stopTestServer', function(done) {
    connect.serverClose();
    done();
});

gulp.task('replaceQuestionPlaceholders', function(done) {
    gulp.src([paths.dist + "questions.html"])
        .pipe(replace('>[percentage]', '>0'))
        .pipe(replace('[question_text]', 'I make decisions quickly'))
        .pipe(replace('[error_message]', 'Please select an option above or this does not apply to continue'))
        .pipe(replace('[button_text]', 'Continue'))
        .pipe(gulp.dest(paths.dist))
        .on("end", done);
});
    
gulp.task('replaceResultsPlaceholders', function(done) {
    gulp.src([paths.dist + "results.html"])
        .pipe(replace('[traits_li_html]', '\u2022 Influencer Some text about the Influencer trait\n\u2022 Driver Some text about the Driver trait\n\u2022 Influencer Some text about the Influencer trait'))
        .pipe(replace('[job_families_li_html]', '\u2022 Influencer Some text about the Influencer trait\n\u2022 Driver Some text about the Driver trait\n\u2022 Influencer Some text about the Influencer trait'))
        .pipe(gulp.dest(paths.dist))
        .on("end", done);
});

gulp.task('lighthousePerformanceTest', function() {
    return gulp.src([paths.performance], {read: false})
        .pipe(mocha({exit: true}))
        .once("error", handleLighthouseError);
});
 
// watches

gulp.task("css:watch", () => gulp.watch([paths.css], gulp.series("min:css")));
gulp.task("sass:watch", () => gulp.watch(paths.scss, gulp.series("sass")));
gulp.task("eslint:watch", () => gulp.watch([paths.js], gulp.series("eslint")));
gulp.task("js:watch", () => gulp.watch([paths.js], gulp.series("js")));
gulp.task("html:watch", () => gulp.watch([paths.html, paths.nunjucks], gulp.series("html")));
gulp.task("images:watch", () => gulp.watch([paths.html], gulp.series("assets")));

// commands

gulp.task("clean", gulp.parallel("clean:js", "clean:css", "clean:assets"));
gulp.task("min", gulp.parallel("min:js", "min:css"));

gulp.task("test", gulp.series("replaceQuestionPlaceholders", "replaceResultsPlaceholders", "startTestServer", "lighthousePerformanceTest", "pa11y", "stopTestServer", "browserStack"));

gulp.task("dev",
    gulp.series(
        "clean",
        "assets",
        "sass",
        "js",
        "html",
        "min:css",
        'headers',
        gulp.parallel(
            "html:watch",
            "css:watch",
            "sass:watch",
            "images:watch",
            "js:watch",
            "connect"))
);

gulp.task("prod",
    gulp.series(
        "clean",
        "assets",
        "sass",
        "js",
        "html",
        "min",
        'rev',
        'headers')
);

gulp.task("default", gulp.series("prod"));