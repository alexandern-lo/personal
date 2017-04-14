# AVEND web client  
## Getting started  
* Clone this repo  
* Run `npm install`  
* Run `npm start`  

App supports vanilla [Redux DevTools](https://github.com/gaearon/redux-devtools) and [Redux DevTools Extension](https://github.com/zalmoxisus/redux-devtools-extension/) [Chrome extension](https://chrome.google.com/webstore/detail/redux-devtools/lmhkpmbekcpmknklioeibfkpmmfibljd)  
We use webpack to build the app and implement hot reloading (you shouldn't reload web page to see your changes)  

## Development  
Don't forget to add git hooks https://github.com/studiomobile/git-hooks  
We use [gitflow strategy](http://nvie.com/posts/a-successful-git-branching-model/). 'master' branch contains production code. 'dev' branch contains latest stable code. You should merge your feature-branches into dev with flag `--no-ff`  

## Deployment  
Run 'npm run build' to get optimized build. Output directory is `./dist`.  
Run 'npm run explore' to explore optimized build. This script will inspect
optimized build and open your default browser with build analyze result (see [source-map-explorer](https://github.com/danvk/source-map-explorer) for details)

## Documentation
To build documentation just run `npm run build-docs`. Docs will be placed in `./docs` folder.
Make sure `esdoc` is installed (`sudo npm install -g esdoc`)
