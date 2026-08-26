# Disallow consecutive empty lines

## Rule Details

This rule reports every empty line that immediately follows another empty
line, so scripts are kept free of large vertical gaps. A line is considered
empty when it contains nothing but whitespace. Single empty lines used to
separate statements are allowed; only the second and subsequent lines in a
run of blank lines are flagged.

This rule is fixable. Running TSQLLint with the `--fix` option removes the
duplicate empty lines, collapsing each run of blank lines down to one.

Examples of **incorrect** code for this rule:

```tsql
SELECT * FROM Foo;


SELECT * FROM Bar;
```

Examples of **correct** code for this rule:

```tsql
SELECT * FROM Foo;

SELECT * FROM Bar;
```
