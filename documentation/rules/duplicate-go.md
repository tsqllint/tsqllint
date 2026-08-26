# Disallow consecutive GO batch separators

## Rule Details

This rule reports a `GO` batch separator that immediately follows another
`GO`. Consecutive `GO` statements produce an empty batch, which serves no
purpose and is usually the result of a copy/paste or merge mistake.
Whitespace, semicolons, and multiline comments between the two `GO`
statements are ignored, so they are still reported as duplicates.

This rule is fixable. Running TSQLLint with the `--fix` option removes the
redundant `GO` statement.

Examples of **incorrect** code for this rule:

```tsql
SELECT * FROM Foo
GO
GO
```

Examples of **correct** code for this rule:

```tsql
SELECT * FROM Foo
GO
```
