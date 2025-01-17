
# c3IDE - CHANGE LOG

## Build 1.1.079
* increment web server port, when port is being used by another process [Issue 66](https://github.com/armandoalonso/c3IDE/issues/66)
* update all addon templates to include this._info.SetCanBeBundled(true); [Issue 64](https://github.com/armandoalonso/c3IDE/issues/64)
* added basic support for timeline integration, added interpolatable property, and get/set function to instance runtime, might improve in future [Issue 65](https://github.com/armandoalonso/c3IDE/issues/65)

### Build 1.1.0.78
* fix issue with dom side script, add new options for other file types
* update templates to work with C3 minifier

### Build 1.1.0.76
* add option to disale code formatting on export/save [Issue 61](https://github.com/armandoalonso/c3IDE/issues/61)
* add extra logging around changing icons to help debug issue [Issue 60] (https://github.com/armandoalonso/c3IDE/issues/60)
* fix bug with change category context menu not saving properly [Issue 58](https://github.com/armandoalonso/c3IDE/issues/58)

### Build 1.1.0.75
* fix bug with importing domside scripts [Issue 55](https://github.com/armandoalonso/c3IDE/issues/55)

### Build# 1.1.0.74
* fix issue with version override not persisting value when entering version manually 

### Build# 1.1.0.71/72/73
* fix issue with duplicate filename when dom script/filedep shared same file [Issue 51](https://github.com/armandoalonso/c3IDE/issues/51)

### Build# 1.1.0.69
* Fix compiled effect addon showing wrong version build version [Issue 45](https://github.com/armandoalonso/c3IDE/issues/48)

### Build# 1.1.0.68
* add option to specify web server fault, deafult port is 8080, if there is any kind of exception, the port will default to 8080. [Issue 45](https://github.com/armandoalonso/c3IDE/issues/45)
* fix bug with c2 behavior import, coming in as a plugin [Issue 47](https://github.com/armandoalonso/c3IDE/issues/47)

### Build# 1.1.0.67
* fix bug with effect and incrementing version number

### Build# 1.1.0.66
* fix bug with combo plugin property and initial value

### Build# 1.1.0.65
* fix param naming convention with c3import, with only c2 runtime. param name was being left with hypen, applied camelCase filter adn remove hypens.
* fix regression with 3rd part files on import. when adding changes for bytes, i missed a section of code for c2only runtime, and no 3rd party files were being populated correctly 
* fix bug with addon validation looking at whole language string instead of just display text
* add code folding to c2 runtime file

### Build# 1.1.0.64
* fix bug with 3rd party files not saving after exiting window
* fix bug with crash after importing file with null highlight

### Build# 1.1.0.63
* fix bug with c3addon import not flagging files correctly, causing null references 
* fix error when leaving desciption blank and creating new addon, then navigating to different tab
* fix error duplicating addon, properties not marked as serializable 

### Build# 1.1.0.60
* fix bug with effect parameters not being marked as serializble, causing crash on duplication 

### Build# 1.1.0.59
* Fix the mime type resolver during file import to refelect the correct mime type (almost everything was being treated as octet stream)
* fix issue with c2 effect import, assign duplicate id to parameters causing unstability 

### Build# 1.1.0.58
* fix bugs with thrid party import, not handling binary files correctly.
* fix performace issue with text editor and large files with no newlines, on import newlines will ge generated on semi-colons, this can be fixed by a new compress flag on export
* added 2 new options to third party import (compress file on export, and plain text)
	- Compress file on export, will remove all comments and white spaces and newlines from the exported file (good for large js files)
	- Plain text - should be populated for most files types, due to the way c3ide is encoding the data, it might generate garbage. this flags allows binary files to treated a stream fo bytes instead of ascii to ensure no data is lost in the final export     

### Build# 1.1.0.56
* update templates to include debugger function on instance
* add support for new async action with is-async flag (actions)
* add construct theme template, (add new css window) [Issue 33](https://github.com/armandoalonso/c3IDE/issues/33)
* create css syntax highlighting def
* add way to change name (context menu) [Issue 32](https://github.com/armandoalonso/c3IDE/issues/32)
* update interaction between global search checkbox, change from double clcik to single click to select  

### Build# 1.1.0.55
* generate uniforms at the top of the file, not on top of main
* add bool keyword to glsh syntax highlighting 

### Build# 1.1.0.54
* added extra shortcuts and context menu options [Issue #25](https://github.com/armandoalonso/c3IDE/issues/25)
* fixed typo durinign addon import [Issue #27](https://github.com/armandoalonso/c3IDE/issues/27)
* fixed issue with impoting c3addon when only c2addon is defined [Issue #26](https://github.com/armandoalonso/c3IDE/issues/26)

### Build# 1.1.0.53
* updated search panel highlight color based on theme
* add cut/copy/paste context menus
* add find / find all context menu
* add comment / uncomment lines context menu

### Build# 1.1.0.52
* sort all auto completion data before being displayed, this should mimic visual studio auto completion abit a more
* implement single window search with word highlighting (ctrl-f) in text editor
* remove javascript compression of 3rd part files, will add this as an option in upcoming release (after issues are fixed)
* add scroll bar to test window to account for scaling
* add wrapping to description textbox when creating addons
* fix plugin property template for group type
* fix issue with auto complete missing some user tokens from the current window

### Build# 1.1.0.50 (04/15/2019)
* implemented code folding for all the javascript editors, code folding is ussing a brace stragety  

### Build# 1.1.0.49 (04/14/2019)
* fix typos in ace
* remove regex highlighting, and fix string highlighting
* fix typo with svg import 

### Build# 1.1.0.47 (04/12/2019)
* add c2 effect import
* fix minor bug with effect compile, where empty c3runtime folder would be created 
* fix bug with syntax highlighting regex it, only on monokai theme, need to verify other themes works fine

### Build# 1.1.0.45 (04/11/2019)
* add option to launch beta when construct is launched from the application
* added two readonly options with the url for bothe construct versions
* added button on the option window to go an update the construct version
* fix bug with auto completion requestin insetion when a symbol when typed
* added option to set auto increment of version when publish c3addon
* hide linting button until implementation is complete 
* start work on navigation pipeline, this will allow the search to navigate to any instance found (still needs work, changes are internal)
* add change author context action to dashboard

### Build# 1.1.0.44 (04/10/2019)
* fix version bug where plugin.js was not being updated
* fix c2 import property of readonly property
* extract full version on import (c3addon)
* add option button to open import log
* fix bug with importing and rotatable flag not exists
* fix file depenceny on import, so they show up correctly on addon.json
* fix import of behaviors with blank category
* ensure only svg icons are used, if png is imported it will be replaced with default svg icon
* fix bug with change addon_id not having c2runtime file indexed caused crash

### Build# 1.1.0.41 (04/08/2019)
* added new option to parse c2 addons using *armaldio's* construct parsing service => https://github.com/WebCreationClub/construct-addon-parser 
* added fall back in cases where the service mgiht fail (no connection) to use edittime ast 
* added import for c3addon (effects) c2 is still a work in progress
* added new option in dashboard context menu ot change addon icon
* added loading overlay to async pasring calls, this loading overlay needs to be reused in sections of the application that migh take time to perform actions

### Build# 1.1.0.40 (04/05/2019)
* basic c2addon import, still buggy, but works some of the time, please reports bugs 
* fixed bugs with third party file and unstable check box for path
* added dropdown for 3rd party file output type
* changed 3rd party file template so now there is better templating when generating files
* BREAKING CHANGE => added newest 3rd party file properties to export, this was not added before, this might break some older exported project, the project might import but not set all the correct settings, if an unexpected error happen file an issue and include the project and i will fix it.
* added validation with passing empty param to file depenency
* changed the way binary files are handled on import
* changed the way large files are handled on import, now large js files will be formatted on import to help the dumb text edit i am using lazy load the text better, on compile all js script will be compressed, this will become a  configurable setting in future update
* added better mime type resolution for erd party files => this also needs to get changed on web server in future update
* effect name & desc now get populated from the uniform name for quicker effect creation
* (hotfix) fix bug with old import that did not have valid extention, crashing app
* fix bug with c2 import and only oneflag defined <> 0

### Build# 1.1.0.35 (04/02/2019)
* add new window for C2RUNTIME.js, import projects with c2runtime now show up here and compile correctly
* added path to thridparty files, this might make some files not show up correctly during compliation, please go back to your third party files and set the correct path of the files
* fix bug when generating propertie/category json, and json was not valid
* make ace list box resizable, now the list box can be resized, before it was tstaic at 300 px
* added better logging during c3addon import, after every import there is a an import.log, this log should have better information about any error that happen during import
* added error handling when logging during web server start/stop command, this had a chance of throwing uncaught exception. *more research is still needed*

### Build# 1.1.0.34 (04/02/2019)
* import : fix issue where ace is only defined for c2runtime, but exists in both (need to add better logging aroudn these edge cases)
* import : fix issue importing async function
* import : dont crash when ace has same function signature defined twice (need better logging aroudn this)
* import : fix issue when aces function where created using older version of javascript (ex. using function keyword)
* performance: cache dashboard filter to stop performance slow down when lotsof addons are imported 
* (hotfix) fix issue with bad startup on new installation

### Build# 1.1.0.32 (04/01/2019)
* remove unsed files during export (ex, aces during effect export)
* add new boolean parameter type for conditions/actions 
* add experimental c3addon import, to import .c3addon file drag into dashboard
* add exception handling for importing c2runtime files (only c3runtime file is supported)
* (hotfix) fix bug when not all aces were defined in ace.json (ex. no expression proeperty defined)
* (hotfix) fix bug when comment is on same line as function declartion 

### Build# 1.1.0.26 (03/31/2019)
* add exception handling to format json, stop crashes
* fix bug with effect category not being displayed correctly
* added new dropdown to change the effect category from the properties window, plugns has access to addon.json so its not needed there
* added logic to sync up version data to addon.json, on version update addon.json now gets updated instantly
* overhauled addon id, now addon id is a seperate property instead of a computed property from author_class,when an addon is created there is a new field for addon id. there is also a new context menu option on the dashboard that lets you change the addon id 
* (hotfix) add backward support for addons that have no defined addon id
* (hotfix) fix bug with importing project with no addon id

### Build# 1.1.0.23 (03/31/2019)
* replace spaces in plugin proeprty id with hypen
* fix order of plugin property, now properties will generate in the order they are added
* make enter key, submit new plugin property on dialog window
* replace spaces in param id with hypen
* escape quotes in param description input
* escape quotes in ace description input

### Build# 1.1.0.22 (03/30/2019) 
* update documentation + release on itch.io
* update aces dialogs now changing id should populate list name/ translated name automatically
* update aces params to populate the param name when id changes

### Build# 1.1.0.19 (03/29/2019) 
* add changelog to solution, fix about window to link to new changelog
* remove old change log button on options window

### Build# 1.1.0.18 (03/29/2019) 
* (hotfix) fix issue with thrid party file being null, when it expected byte array
* disable linting, while a new version is being developed (current version was not very usbale), going to be using esprima to create javascript AST and go from there
* add commands for actions hidden in context menus
* add fix to application start up, when opening directly from c3ide project file, now you should be get prompt  
* change order of dashboadr, now last modified items will always be at the top 
* fix bug with pinning not being respected when restarting the app
* add about window to main menu, window has version information, and links 
* add option to remove console.log statements on compile (only comments them out);

### Build# 1.1.0.8 (03/29/2019)
* add search/filter to dashboard
* display ace category next to ace id in listbox 

### Build# 1.1.0.7 (03/28/2019)
* add option to export project as mutiple files for version control / diff
* add option to overwrite addon id on import
* added better version tracking, update test window to show major/minor/revision/build, increment on c3addon build
 
### Build# 1.1.0.6 (03/28/2019)
* fix issue with effect bool casing

### Build# 1.1.0.5 (03/27/2019)
* re-structure the addon creation process - now there are 2 seperate windows one for the dashboard and one for the creation of the addon.
* fix bug with gloabl search and replace, not finding text in the current window.
* change how 3rd party files are imported, so now non-text files are accepted and converted to base64.
* add very simple js linting (not working as expected, it needs love) 
* re implement effect pages to streamline the creation of effects
* implement a menu manager, we no longer use to isolated menus (effect/main), menu is now controlled by the manager
* fix issue with adding params being sorted in inverse ordder

### Build# 1.0.1.27 (02/17/2019)
* fix issue with javascript arrow function and js beauitifer.
* default template now dont contain any pre-defined properties 

### Build# 1.0.1.25 (02/12/19)
* add variadic parameter to action type (this feature is undocumented in c3 sdk but might be useful) using it comes with a compiler warning
* added add param to context menu in list box for all ace types

### Build# 1.0.1.24 (02/12/19)
* fix bug with empty string param values
* fix bug with search not looking at current file being searched

### Build# 1.0.1.23 (02/11/19)
* add global search and replace (highlight word and hit F1 to bring up the search and replace window) (effect addon does not have search and replace yet, this will be in the next update, where i will focus a bit more on the effect workflow)
* fixed an issue with the export
* moved the export buttons to the test windows (this ensure that you can only export the currently loaded addon)
* fixed issue with aces not saving properly when navigating the windows
* forced class name to not have spaces on creation
* removed update button while i rework a better way to replace addon metadata. this should be much easier now that i have the search and replace system in place

### Build# 1.0.1.22 (02/07/19)
* fix issue with update plugin when values were null or empty
* enforce all addon metadata values to not be empty (description was previously nullable which caused issues down stream)
* fix validate addon for effects
* include addon validator logic in the compilation 
* fix issue with c3addon path being readonly, it is no longer readonly

### Build# 1.0.1.20 (02/06/19)
* added extra buttons to compile window, to start web server/open construct, refactored how compile logging wroks 
* remove spaces from author, since it's used as part of the addon id   

### Build# 1.0.1.17 (02/06/19)
* add compile on save option - this option will compile the selected addon when a change is saved (ctrl-s), this will bring up a compilation log popout window, this allows for quicker iteration. you can go in start the web server, start making changes and compile on the fly, this avoids having to manually recompile the files everytime.    

#### Build# 1.0.1.16 (02/05/19) 
* add new c3ide icons and file association, now there are 2 new ways to pass arguments into the application, it will accept a path to .c3ide file or a guid to identify the addon to load. 
* added option to pin menu, which will leave menu open 

#### Build# 1.0.1.3  (02/02/2019)
* refactor auto completion, there were some issue that need to be fixed, as a quick fix, i disbaled the embedded documentaion until i can recolve the issue,. as of now auto completion still works but its not context based. it works more like how sublime text works.
* updated syntax highlighting across the board for all themes, now json and js are highlighted differently
* added ayu light and ayu mirage themesn (https://github.com/ayu-theme/ayu-colors )
* added effect template, and changed parts of the ui to be dynamic based on addon type
* updated the addon compiler to support effects
* start working on the infustrature to create find and replace, and improve the editors workflow
* added a label on the dashboard to show plugin type
* fix issue with aces category being overwritten

#### Build# 1.0.1.2  (01/29/2019)
* (hotfix) fix issue with start server 

#### Build# 1.0.1.1  (01/29/2019)
* Fixed bug with addon compiler, now the process will terminate when there is an error
* Added a validate addon button test window, (as of now this only check json)
* Fixed bug when navigating from the aces windows, sometimes the changes were not being saved
* Added a compile only button to test window, which just regenerates the addon without starting the server
* Added a start web server button to test window, which just starts the web server with no compilation 
* Started working on eventing backend (not user facing)

#### Build# 1.0.1.0  (01/27/2019)
* upgrade minor build, for newer build numbers
* (hotfix) now when you duplicate aces, ace/lang scriptName property also get updated 

#### Build# 1.0.0.19  (01/27/2019)
* Added duplicate action function to context menu.
* Added duplicate condition function to context menu.
* Added duplicate expression function to context menu.
