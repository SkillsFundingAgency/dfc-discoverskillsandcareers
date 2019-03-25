/* eslint-disable no-console */
"use strict";

// helpers

function pa11yErrorHandler() {
    process.exit(1);
}

function lighthouseErrorHandler() {
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
    merge = require('merge-stream'),
    babel = require("gulp-babel"),
    autoprefixer = require('gulp-autoprefixer'),
    standard = require('gulp-standard'),
    browserify = require('gulp-browserify');

// paths

var paths = {
    templatesSrc: "ViewsDev/",
    src: "src/",
    dist: "wwwroot/",
    templatesDist: "Views/",
    temp: ".temp/",
    tests: "specs/",
    conf: "conf/"
};

// paths – input

paths.html = paths.templatesSrc + "templates/**/*.cshtml";
paths.nunjucks = paths.templatesSrc + "partials/**/*.njk";
paths.scss = paths.src + "scss/**/*.scss";
paths.images = paths.src + "images/**/*";
paths.js = paths.src + "js/**/*.js";
paths.favicon = paths.src + "favicon.ico"

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
paths.browserStackConf = paths.conf + "conf.js";
paths.browserStackSpec = paths.tests + "browser.spec.js";

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
        .pipe(browserify())
        .pipe(babel())
        .pipe(filter(['**/*', '!**/modules/*']))
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
          ext: '.cshtml',
          path: [
            "node_modules/govuk-frontend/",
            "node_modules/govuk-frontend/components/",
            paths.templatesSrc + "templates/",
            paths.templatesSrc + "partials/"]
        }))
        .pipe(gulp.dest(paths.templatesDist))
        .pipe(connect.reload());
});

gulp.task('favicon', function() {
    return gulp.src(paths.favicon).pipe(gulp.dest(paths.dist))
})

gulp.task('rev', () => {
    const assetFilter = filter(['**/*', '!**/*.html', '!**/*.woff*', '!**/*.eot', '!**/*.ico'], { restore: true });

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

// QA

gulp.task('pa11y', function() {
    return gulp.src(paths.accessibilty, {read: false})
        .pipe(mocha({exit: true}))
        .on("error", pa11yErrorHandler);
});

gulp.task('browserStack', function(done) {
    gulp.src([paths.browserStackSpec])
        .pipe(protractor({
            configFile: paths.browserStackConf
        }))
        .on("end", done);
});

gulp.task('lighthousePerformanceTest', function() {
    return gulp.src([paths.performance], {read: false})
        .pipe(mocha({exit: true}))
        .on("error", lighthouseErrorHandler);
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

gulp.task("test", gulp.series("pa11y", "lighthousePerformanceTest"));

gulp.task("dev",
    gulp.series(
        "clean",
        "assets",
        "sass",
        "js",
        "html",
        "min:css",
        'headers',
        'favicon',
        gulp.parallel(
            "html:watch",
            "css:watch",
            "sass:watch",
            "images:watch",
            "js:watch"))
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
        'headers',
        'favicon')
);

gulp.task("default", gulp.series("prod"));
