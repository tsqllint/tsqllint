# Change Log

## [Unreleased](https://github.com/tsqllint/tsqllint/tree/HEAD)

[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.10.0...HEAD)

**Merged pull requests:**

- Rename compatability\_level for consistency [\#167](https://github.com/tsqllint/tsqllint/pull/167) ([nathan-boyd](https://github.com/nathan-boyd))

## [v1.10.0](https://github.com/tsqllint/tsqllint/tree/v1.10.0) (2018-04-12)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.9.4...v1.10.0)

**Merged pull requests:**

- Add Configurable SQL Compatibility Level [\#166](https://github.com/tsqllint/tsqllint/pull/166) ([nathan-boyd](https://github.com/nathan-boyd))
- Add documentation for disallow-cursors rule [\#164](https://github.com/tsqllint/tsqllint/pull/164) ([nathan-boyd](https://github.com/nathan-boyd))
-  Add documentation for data-type-length rule [\#163](https://github.com/tsqllint/tsqllint/pull/163) ([nathan-boyd](https://github.com/nathan-boyd))
- Fix data compression rule documentation [\#162](https://github.com/tsqllint/tsqllint/pull/162) ([nathan-boyd](https://github.com/nathan-boyd))
- Add documentation for data-compression rule [\#161](https://github.com/tsqllint/tsqllint/pull/161) ([nathan-boyd](https://github.com/nathan-boyd))
- Add documentation for cross-database-transaction rule [\#160](https://github.com/tsqllint/tsqllint/pull/160) ([nathan-boyd](https://github.com/nathan-boyd))
- Add documentation for condition-begin-end rule [\#159](https://github.com/tsqllint/tsqllint/pull/159) ([nathan-boyd](https://github.com/nathan-boyd))
- Add rule for named constraints in temp table [\#155](https://github.com/tsqllint/tsqllint/pull/155) ([nathan-boyd](https://github.com/nathan-boyd))

## [v1.9.4](https://github.com/tsqllint/tsqllint/tree/v1.9.4) (2018-02-14)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.9.3...v1.9.4)

**Fixed bugs:**

- TSqlLint fails \(report errors\) even is disabled in 1st line by  /\* tsqllint-disable \*/ if file contain "\" \(backslash\) symbol [\#152](https://github.com/tsqllint/tsqllint/issues/152)

**Closed issues:**

- Plugins should be given the option to honor inline disablements [\#142](https://github.com/tsqllint/tsqllint/issues/142)

**Merged pull requests:**

- Update to not set error code when file does not parse [\#157](https://github.com/tsqllint/tsqllint/pull/157) ([nathan-boyd](https://github.com/nathan-boyd))

## [v1.9.3](https://github.com/tsqllint/tsqllint/tree/v1.9.3) (2018-02-09)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.9.2...v1.9.3)

**Merged pull requests:**

- Update Fragment builder and Configs [\#153](https://github.com/tsqllint/tsqllint/pull/153) ([nathan-boyd](https://github.com/nathan-boyd))

## [v1.9.2](https://github.com/tsqllint/tsqllint/tree/v1.9.2) (2018-02-08)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.9.1...v1.9.2)

**Implemented enhancements:**

- Order error output by row number [\#130](https://github.com/tsqllint/tsqllint/issues/130)

**Merged pull requests:**

- Update Parse Error Handling To Support Global Rule Ignores [\#151](https://github.com/tsqllint/tsqllint/pull/151) ([nathan-boyd](https://github.com/nathan-boyd))
- 130 outputsort it [\#150](https://github.com/tsqllint/tsqllint/pull/150) ([danielgasperut](https://github.com/danielgasperut))

## [v1.9.1](https://github.com/tsqllint/tsqllint/tree/v1.9.1) (2018-02-04)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.9.0...v1.9.1)

**Merged pull requests:**

- Add Check for null SqlFragment in Parser [\#149](https://github.com/tsqllint/tsqllint/pull/149) ([nathan-boyd](https://github.com/nathan-boyd))

## [v1.9.0](https://github.com/tsqllint/tsqllint/tree/v1.9.0) (2018-02-03)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.8.10...v1.9.0)

**Implemented enhancements:**

- Rule for End Catch; [\#131](https://github.com/tsqllint/tsqllint/issues/131)
- Visual Studio Extension [\#116](https://github.com/tsqllint/tsqllint/issues/116)

**Merged pull requests:**

- Document config file discovery features in readme [\#148](https://github.com/tsqllint/tsqllint/pull/148) ([nathan-boyd](https://github.com/nathan-boyd))
- Add acceptance testing scripts  [\#147](https://github.com/tsqllint/tsqllint/pull/147) ([nathan-boyd](https://github.com/nathan-boyd))
- Set global rule exception name to GLOBAL [\#146](https://github.com/tsqllint/tsqllint/pull/146) ([nathan-boyd](https://github.com/nathan-boyd))
- Pass rule exceptions to plugins [\#145](https://github.com/tsqllint/tsqllint/pull/145) ([nathan-boyd](https://github.com/nathan-boyd))
- Read config file path from environment variable [\#144](https://github.com/tsqllint/tsqllint/pull/144) ([nathan-boyd](https://github.com/nathan-boyd))
- Add Change Log [\#141](https://github.com/tsqllint/tsqllint/pull/141) ([nathan-boyd](https://github.com/nathan-boyd))
- Get config from local directory [\#138](https://github.com/tsqllint/tsqllint/pull/138) ([nathan-boyd](https://github.com/nathan-boyd))
- Remove requirement for ELSE statements to be terminated with semicolons [\#134](https://github.com/tsqllint/tsqllint/pull/134) ([nathan-boyd](https://github.com/nathan-boyd))
- Issue 131 END CATCH; [\#132](https://github.com/tsqllint/tsqllint/pull/132) ([danielgasperut](https://github.com/danielgasperut))
- Allow files with syntax errors to be linted [\#129](https://github.com/tsqllint/tsqllint/pull/129) ([ostreifel](https://github.com/ostreifel))

## [v1.8.10](https://github.com/tsqllint/tsqllint/tree/v1.8.10) (2018-01-12)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.8.9...v1.8.10)

**Merged pull requests:**

- Update tsqllint.js to emit exit code from wrapped dotnet call [\#126](https://github.com/tsqllint/tsqllint/pull/126) ([nathan-boyd](https://github.com/nathan-boyd))

## [v1.8.9](https://github.com/tsqllint/tsqllint/tree/v1.8.9) (2018-01-11)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.8.8...v1.8.9)

**Fixed bugs:**

- When Passed Invalid SQL TSQLLint Reports Error But Does not Return non-null Status Code [\#124](https://github.com/tsqllint/tsqllint/issues/124)

**Merged pull requests:**

- Update to return 1 when passed invalid SQL [\#125](https://github.com/tsqllint/tsqllint/pull/125) ([nathan-boyd](https://github.com/nathan-boyd))
- Create more tests for inline rule disablements  [\#123](https://github.com/tsqllint/tsqllint/pull/123) ([nathan-boyd](https://github.com/nathan-boyd))

## [v1.8.8](https://github.com/tsqllint/tsqllint/tree/v1.8.8) (2018-01-09)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.8.7...v1.8.8)

**Merged pull requests:**

- another reference to x32 [\#122](https://github.com/tsqllint/tsqllint/pull/122) ([danielgasperut](https://github.com/danielgasperut))

## [v1.8.7](https://github.com/tsqllint/tsqllint/tree/v1.8.7) (2018-01-09)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.8.6...v1.8.7)

**Merged pull requests:**

- pr process.arch bug in setup script.  [\#121](https://github.com/tsqllint/tsqllint/pull/121) ([danielgasperut](https://github.com/danielgasperut))
- Modify JS to use syntax for Node.js 4+ [\#120](https://github.com/tsqllint/tsqllint/pull/120) ([dougwilson](https://github.com/dougwilson))

## [v1.8.6](https://github.com/tsqllint/tsqllint/tree/v1.8.6) (2017-12-30)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.8.5...v1.8.6)

## [v1.8.5](https://github.com/tsqllint/tsqllint/tree/v1.8.5) (2017-12-30)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.8.4...v1.8.5)

**Fixed bugs:**

- Incorrect Semicolon Termination Within Waitfor [\#117](https://github.com/tsqllint/tsqllint/issues/117)

**Closed issues:**

- Relax transaction isolation level read uncommitted for "table scripts" [\#65](https://github.com/tsqllint/tsqllint/issues/65)

**Merged pull requests:**

- Fix Semicolon Termination Waitfor Bug [\#118](https://github.com/tsqllint/tsqllint/pull/118) ([nathan-boyd](https://github.com/nathan-boyd))
- Add standard and update js to comply [\#115](https://github.com/tsqllint/tsqllint/pull/115) ([nathan-boyd](https://github.com/nathan-boyd))
- Update appveyor to publish tags only [\#114](https://github.com/tsqllint/tsqllint/pull/114) ([nathan-boyd](https://github.com/nathan-boyd))
- Implement Stylecop Analyzer [\#113](https://github.com/tsqllint/tsqllint/pull/113) ([nathan-boyd](https://github.com/nathan-boyd))
- Update dependecies [\#112](https://github.com/tsqllint/tsqllint/pull/112) ([nathan-boyd](https://github.com/nathan-boyd))
- Update dependencies and reduce complexity [\#111](https://github.com/tsqllint/tsqllint/pull/111) ([nathan-boyd](https://github.com/nathan-boyd))
- Remove unused templates, configs, and files [\#110](https://github.com/tsqllint/tsqllint/pull/110) ([nathan-boyd](https://github.com/nathan-boyd))
- Remove .net framework from build targets [\#109](https://github.com/tsqllint/tsqllint/pull/109) ([nathan-boyd](https://github.com/nathan-boyd))
- Tooling Updates [\#108](https://github.com/tsqllint/tsqllint/pull/108) ([nathan-boyd](https://github.com/nathan-boyd))
- Add versioning script [\#107](https://github.com/tsqllint/tsqllint/pull/107) ([nathan-boyd](https://github.com/nathan-boyd))

## [v1.8.4](https://github.com/tsqllint/tsqllint/tree/v1.8.4) (2017-12-12)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.8.2...v1.8.4)

**Implemented enhancements:**

- The application is limited by the version of .net framework installed on host machine [\#100](https://github.com/tsqllint/tsqllint/issues/100)
- Add a linting rule for nvachar / varchar concatenation  [\#64](https://github.com/tsqllint/tsqllint/issues/64)

**Fixed bugs:**

- Linter expect semicolons after goto 'Label:' [\#97](https://github.com/tsqllint/tsqllint/issues/97)
- Microsoft.SqlServer.TransactSql.ScriptDom throws stack overflow for visitors accepting fragments with large numbers of tokens. [\#95](https://github.com/tsqllint/tsqllint/issues/95)
- Concat strings rule Object reference not set to an instance of an object. [\#88](https://github.com/tsqllint/tsqllint/issues/88)

**Merged pull requests:**

- Create release from Appveyor [\#105](https://github.com/tsqllint/tsqllint/pull/105) ([nathan-boyd](https://github.com/nathan-boyd))
- Update install script to rely upon self contained deployment [\#104](https://github.com/tsqllint/tsqllint/pull/104) ([nathan-boyd](https://github.com/nathan-boyd))
- Remove unused scripts [\#103](https://github.com/tsqllint/tsqllint/pull/103) ([nathan-boyd](https://github.com/nathan-boyd))
- Upgrade to net47 [\#101](https://github.com/tsqllint/tsqllint/pull/101) ([nathan-boyd](https://github.com/nathan-boyd))
- Update Code Coverage Checks and Reporting [\#99](https://github.com/tsqllint/tsqllint/pull/99) ([nathan-boyd](https://github.com/nathan-boyd))
- Add exclusion for Goto & Label Statements to SemicolonTermination Rule [\#98](https://github.com/tsqllint/tsqllint/pull/98) ([nathan-boyd](https://github.com/nathan-boyd))
- Add functional test for plugins [\#96](https://github.com/tsqllint/tsqllint/pull/96) ([nathan-boyd](https://github.com/nathan-boyd))
- Update dependencies [\#94](https://github.com/tsqllint/tsqllint/pull/94) ([nathan-boyd](https://github.com/nathan-boyd))
- New rule to check for unicode strings [\#93](https://github.com/tsqllint/tsqllint/pull/93) ([geoffbaker](https://github.com/geoffbaker))

## [v1.8.2](https://github.com/tsqllint/tsqllint/tree/v1.8.2) (2017-11-30)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.8.1...v1.8.2)

**Closed issues:**

- upper-lower rule incorrectly flags situations where case matters \(despite case-insensitive mode\) [\#90](https://github.com/tsqllint/tsqllint/issues/90)

**Merged pull requests:**

- Update upper-lower rule to only flag comparisons in select statements [\#91](https://github.com/tsqllint/tsqllint/pull/91) ([nathan-boyd](https://github.com/nathan-boyd))

## [v1.8.1](https://github.com/tsqllint/tsqllint/tree/v1.8.1) (2017-11-30)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.8.0...v1.8.1)

**Merged pull requests:**

- Update cross-database-transaction to handle uncommitted transactions [\#89](https://github.com/tsqllint/tsqllint/pull/89) ([nathan-boyd](https://github.com/nathan-boyd))

## [v1.8.0](https://github.com/tsqllint/tsqllint/tree/v1.8.0) (2017-11-29)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.7.0...v1.8.0)

**Merged pull requests:**

- Update README code blocks to display syntax highlighting [\#87](https://github.com/tsqllint/tsqllint/pull/87) ([nathan-boyd](https://github.com/nathan-boyd))
- Concat strings rule [\#84](https://github.com/tsqllint/tsqllint/pull/84) ([geoffbaker](https://github.com/geoffbaker))

## [v1.7.0](https://github.com/tsqllint/tsqllint/tree/v1.7.0) (2017-11-28)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.6.1...v1.7.0)

**Fixed bugs:**

- Stodout buffer too small, truncating output [\#78](https://github.com/tsqllint/tsqllint/issues/78)

**Merged pull requests:**

- Update non-sargable rule to allow isnull when other predicates exist [\#85](https://github.com/tsqllint/tsqllint/pull/85) ([nathan-boyd](https://github.com/nathan-boyd))
- Improve rule visitor builder performance [\#83](https://github.com/tsqllint/tsqllint/pull/83) ([nathan-boyd](https://github.com/nathan-boyd))
- Update full text rule column source [\#82](https://github.com/tsqllint/tsqllint/pull/82) ([nathan-boyd](https://github.com/nathan-boyd))
- Add Full Text Rule [\#81](https://github.com/tsqllint/tsqllint/pull/81) ([nathan-boyd](https://github.com/nathan-boyd))
- Update CommandLineOptionsTests file paths to work cross platform [\#80](https://github.com/tsqllint/tsqllint/pull/80) ([nathan-boyd](https://github.com/nathan-boyd))
- Remove newlines from plugin reporting [\#79](https://github.com/tsqllint/tsqllint/pull/79) ([nathan-boyd](https://github.com/nathan-boyd))

## [v1.6.1](https://github.com/tsqllint/tsqllint/tree/v1.6.1) (2017-11-17)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.6.0...v1.6.1)

## [v1.6.0](https://github.com/tsqllint/tsqllint/tree/v1.6.0) (2017-11-17)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.5.0...v1.6.0)

**Implemented enhancements:**

- Don't allow duplicate plugins to be loaded [\#62](https://github.com/tsqllint/tsqllint/issues/62)
- add relative path support for plugins [\#54](https://github.com/tsqllint/tsqllint/issues/54)
- Port to dotnet standard [\#73](https://github.com/tsqllint/tsqllint/pull/73) ([nathan-boyd](https://github.com/nathan-boyd))

**Closed issues:**

- Enable cross platform installation [\#75](https://github.com/tsqllint/tsqllint/issues/75)
- TSQL Lint node pkg expects win32 \(not installable on OS X 'darwin'\) [\#74](https://github.com/tsqllint/tsqllint/issues/74)

**Merged pull requests:**

- Update build and install process to support osx [\#77](https://github.com/tsqllint/tsqllint/pull/77) ([nathan-boyd](https://github.com/nathan-boyd))
- Update coverage script for dotnet core [\#76](https://github.com/tsqllint/tsqllint/pull/76) ([nathan-boyd](https://github.com/nathan-boyd))
- Does not load a plugin in twice [\#72](https://github.com/tsqllint/tsqllint/pull/72) ([geoffbaker](https://github.com/geoffbaker))
-  Remove un-needed public methods from SQLFileProcessor [\#71](https://github.com/tsqllint/tsqllint/pull/71) ([nathan-boyd](https://github.com/nathan-boyd))
- Remove unused constructor in SqlFileProcessor [\#70](https://github.com/tsqllint/tsqllint/pull/70) ([nathan-boyd](https://github.com/nathan-boyd))
- Improve RuleExceptionFinder Code Coverage [\#69](https://github.com/tsqllint/tsqllint/pull/69) ([nathan-boyd](https://github.com/nathan-boyd))
- Refactor Command Line Parameter Parsing [\#68](https://github.com/tsqllint/tsqllint/pull/68) ([nathan-boyd](https://github.com/nathan-boyd))
- Refactor to Prepare for Strategy Pattern Implementation [\#66](https://github.com/tsqllint/tsqllint/pull/66) ([nathan-boyd](https://github.com/nathan-boyd))

## [v1.5.0](https://github.com/tsqllint/tsqllint/tree/v1.5.0) (2017-11-10)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.4.1...v1.5.0)

**Implemented enhancements:**

- The init and force options display help even if they work as expected [\#59](https://github.com/tsqllint/tsqllint/issues/59)
- Add feature to display configured plugins [\#58](https://github.com/tsqllint/tsqllint/issues/58)
- Add ability to turn off rules in source files [\#29](https://github.com/tsqllint/tsqllint/issues/29)

**Closed issues:**

- Process exit-code 0 with errors in syntax [\#53](https://github.com/tsqllint/tsqllint/issues/53)
- CrossDb rule fires on LinkedServer [\#52](https://github.com/tsqllint/tsqllint/issues/52)

**Merged pull requests:**

- Added support for relative paths in plugin paths [\#63](https://github.com/tsqllint/tsqllint/pull/63) ([geoffbaker](https://github.com/geoffbaker))
- Ability to list plugins [\#61](https://github.com/tsqllint/tsqllint/pull/61) ([geoffbaker](https://github.com/geoffbaker))
- Init/Force changes [\#60](https://github.com/tsqllint/tsqllint/pull/60) ([geoffbaker](https://github.com/geoffbaker))
- Report failure to find plugin [\#56](https://github.com/tsqllint/tsqllint/pull/56) ([jamisonr](https://github.com/jamisonr))
- add TSQLLint package artifacts to .gitignore [\#55](https://github.com/tsqllint/tsqllint/pull/55) ([jamisonr](https://github.com/jamisonr))

## [v1.4.1](https://github.com/tsqllint/tsqllint/tree/v1.4.1) (2017-10-19)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.4.0...v1.4.1)

**Implemented enhancements:**

- create rule for linked server calls [\#50](https://github.com/tsqllint/tsqllint/issues/50)

## [v1.4.0](https://github.com/tsqllint/tsqllint/tree/v1.4.0) (2017-10-14)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.3.0...v1.4.0)

## [v1.3.0](https://github.com/tsqllint/tsqllint/tree/v1.3.0) (2017-10-11)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.2.0...v1.3.0)

**Implemented enhancements:**

- Linter shouldn't expect semicolons after "BEGIN" [\#51](https://github.com/tsqllint/tsqllint/issues/51)

## [v1.2.0](https://github.com/tsqllint/tsqllint/tree/v1.2.0) (2017-10-08)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.1.5...v1.2.0)

**Implemented enhancements:**

- Consider making easier to use in Script Review process [\#42](https://github.com/tsqllint/tsqllint/issues/42)
- Implement Self Init [\#25](https://github.com/tsqllint/tsqllint/issues/25)

**Merged pull requests:**

- Standardize styling [\#49](https://github.com/tsqllint/tsqllint/pull/49) ([nathan-boyd](https://github.com/nathan-boyd))
- Add plugin framework [\#48](https://github.com/tsqllint/tsqllint/pull/48) ([nathan-boyd](https://github.com/nathan-boyd))
- Automatically create config file [\#47](https://github.com/tsqllint/tsqllint/pull/47) ([geoffbaker](https://github.com/geoffbaker))

## [v1.1.5](https://github.com/tsqllint/tsqllint/tree/v1.1.5) (2017-09-18)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.1.4...v1.1.5)

**Fixed bugs:**

- IDENTITY\_INSERT Capitalization not handled correctly [\#44](https://github.com/tsqllint/tsqllint/issues/44)
- Incorrect semicolon-termination detection [\#43](https://github.com/tsqllint/tsqllint/issues/43)
- Crash when given a path to a non-existent directory [\#38](https://github.com/tsqllint/tsqllint/issues/38)

**Closed issues:**

- Illegal Unexpected Token  [\#33](https://github.com/tsqllint/tsqllint/issues/33)

**Merged pull requests:**

- Add set variable rule [\#40](https://github.com/tsqllint/tsqllint/pull/40) ([nathan-boyd](https://github.com/nathan-boyd))
- Fix directory doesnt exist bug [\#39](https://github.com/tsqllint/tsqllint/pull/39) ([nathan-boyd](https://github.com/nathan-boyd))

## [v1.1.4](https://github.com/tsqllint/tsqllint/tree/v1.1.4) (2017-08-30)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.1.3...v1.1.4)

**Implemented enhancements:**

- Implement return codes [\#24](https://github.com/tsqllint/tsqllint/issues/24)

**Merged pull requests:**

- Fix references to APPDATA [\#36](https://github.com/tsqllint/tsqllint/pull/36) ([dougwilson](https://github.com/dougwilson))
- Specify node engine in package.json [\#35](https://github.com/tsqllint/tsqllint/pull/35) ([nathan-boyd](https://github.com/nathan-boyd))
- Add more instructions to installation [\#34](https://github.com/tsqllint/tsqllint/pull/34) ([dougwilson](https://github.com/dougwilson))
- Stylecop [\#32](https://github.com/tsqllint/tsqllint/pull/32) ([geoffbaker](https://github.com/geoffbaker))
- Fix exit code for help option [\#30](https://github.com/tsqllint/tsqllint/pull/30) ([nathan-boyd](https://github.com/nathan-boyd))

## [v1.1.3](https://github.com/tsqllint/tsqllint/tree/v1.1.3) (2017-08-24)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.1.2...v1.1.3)

**Fixed bugs:**

- Post Install Script Doesn't Work With NPM prefix other than AppData -\> Roaming [\#27](https://github.com/tsqllint/tsqllint/issues/27)

**Merged pull requests:**

- Use npm config prefix for postinstall script [\#28](https://github.com/tsqllint/tsqllint/pull/28) ([dougwilson](https://github.com/dougwilson))
- Wildcards [\#26](https://github.com/tsqllint/tsqllint/pull/26) ([geoffbaker](https://github.com/geoffbaker))

## [v1.1.2](https://github.com/tsqllint/tsqllint/tree/v1.1.2) (2017-08-16)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.1.1...v1.1.2)

**Fixed bugs:**

- ScriptDom Not Found [\#21](https://github.com/tsqllint/tsqllint/issues/21)
- Git Bash Shell NPM installation doesn't work out of the box in Windows [\#16](https://github.com/tsqllint/tsqllint/issues/16)
- Install ScriptDom dependency when it doesnt exist on host machine [\#22](https://github.com/tsqllint/tsqllint/pull/22) ([nathan-boyd](https://github.com/nathan-boyd))

**Merged pull requests:**

- Build package in CI [\#20](https://github.com/tsqllint/tsqllint/pull/20) ([nathan-boyd](https://github.com/nathan-boyd))
- Fix Windows Git Bash Install [\#17](https://github.com/tsqllint/tsqllint/pull/17) ([nathan-boyd](https://github.com/nathan-boyd))
- Improve error reporting [\#15](https://github.com/tsqllint/tsqllint/pull/15) ([nathan-boyd](https://github.com/nathan-boyd))
- Use repository short-hand in package.json [\#14](https://github.com/tsqllint/tsqllint/pull/14) ([dougwilson](https://github.com/dougwilson))
- Cache the final NuGet package installed files [\#13](https://github.com/tsqllint/tsqllint/pull/13) ([dougwilson](https://github.com/dougwilson))
- Remove the NuGet IP from AppVeyor [\#12](https://github.com/tsqllint/tsqllint/pull/12) ([dougwilson](https://github.com/dougwilson))
- Fix syntax error to show "error" instead of "off" [\#11](https://github.com/tsqllint/tsqllint/pull/11) ([dougwilson](https://github.com/dougwilson))

## [v1.1.1](https://github.com/tsqllint/tsqllint/tree/v1.1.1) (2017-08-04)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.1.0...v1.1.1)

**Fixed bugs:**

- CTE's used with derived table asking for aliasing. [\#9](https://github.com/tsqllint/tsqllint/issues/9)
- Fix bug with MultiTableAliasRule and CTE [\#10](https://github.com/tsqllint/tsqllint/pull/10) ([nathan-boyd](https://github.com/nathan-boyd))

## [v1.1.0](https://github.com/tsqllint/tsqllint/tree/v1.1.0) (2017-07-28)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.0.2...v1.1.0)

**Implemented enhancements:**

- add guard rail to prevent clobbering pre-existing config file when using init command line option [\#7](https://github.com/tsqllint/tsqllint/issues/7)

## [v1.0.2](https://github.com/tsqllint/tsqllint/tree/v1.0.2) (2017-07-25)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.0.1...v1.0.2)

## [v1.0.1](https://github.com/tsqllint/tsqllint/tree/v1.0.1) (2017-07-24)
[Full Changelog](https://github.com/tsqllint/tsqllint/compare/v1.0.0...v1.0.1)

## [v1.0.0](https://github.com/tsqllint/tsqllint/tree/v1.0.0) (2017-07-23)
**Implemented enhancements:**

- update config file creation to write file to users directory [\#6](https://github.com/tsqllint/tsqllint/issues/6)
- Add new rules to integration test [\#5](https://github.com/tsqllint/tsqllint/issues/5)
- add code coverage tooling and reporting [\#2](https://github.com/tsqllint/tsqllint/issues/2)
- need better .gitignore [\#1](https://github.com/tsqllint/tsqllint/issues/1)



\* *This Change Log was automatically generated by [github_changelog_generator](https://github.com/skywinder/Github-Changelog-Generator)*