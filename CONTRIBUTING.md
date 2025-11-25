# Contributing to OneCiel.Core.Dynamics

We appreciate your interest in contributing to OneCiel.Core.Dynamics!

## Code of Conduct

- Be respectful and inclusive
- Provide constructive feedback
- Focus on the code, not the person

## Development Setup

1. Clone the repository
2. Open the solution in Visual Studio
3. Build the solution: `dotnet build`

## Making Changes

### Commit Messages

Follow conventional commit format:
```
type(scope): description

Detailed explanation if needed

Fixes #issue-number
```

Types: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`

### Code Style

- Use C# 11+ features appropriately
- Follow Microsoft naming conventions
- Use meaningful variable names
- Add XML documentation comments for public APIs
- Keep lines under 120 characters when practical
- Use LINQ where appropriate but prioritize readability

## Documentation

- Update XML comments when modifying APIs
- Update README.md for user-facing changes
- Include examples for new features
- Keep documentation in English

## Submitting Changes

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Make your changes
4. Commit with clear messages
5. Push to your fork
6. Create a Pull Request with clear description

## Release Checklist

Before publishing, verify:

- All code compiles without warnings
- CHANGELOG.md is updated
- Version numbers match planned release
- README files are up to date
- XML documentation is complete
- No sensitive information in code
- All public APIs are documented

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

