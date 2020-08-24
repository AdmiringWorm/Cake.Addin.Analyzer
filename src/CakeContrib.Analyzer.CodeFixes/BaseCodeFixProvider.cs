namespace CakeContrib.Analyzer.CodeFixes
{
	using System;
	using System.Collections.Immutable;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CodeFixes;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using Microsoft.CodeAnalysis.Simplification;

	public abstract class BaseCodeFixProvider : CodeFixProvider
	{
		private readonly ImmutableArray<string> _fixableDiagnosticIds;

		protected BaseCodeFixProvider(params string[] identifiers)
		{
			this._fixableDiagnosticIds = ImmutableArray.Create(identifiers);
		}

		public override sealed ImmutableArray<string> FixableDiagnosticIds => this._fixableDiagnosticIds;

		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

		protected static NameSyntax BuildQualifiedName(params string[] names)
		{
			if (names.Length == 0)
			{
				throw new ArgumentOutOfRangeException(nameof(names), "We need at least 1 name to build the correct same syntax");
			}
			else if (names.Length == 1)
			{
				return SyntaxFactory.IdentifierName(names[0]);
			}

			NameSyntax? qualifiedName = default;

			foreach (var name in names)
			{
				var identifierName = SyntaxFactory.IdentifierName(name);
				if (qualifiedName is null)
				{
					qualifiedName = identifierName;
				}
				else
				{
					qualifiedName = SyntaxFactory.QualifiedName(qualifiedName, identifierName)
						.WithAdditionalAnnotations(Simplifier.Annotation);
				}
			}

#pragma warning disable CS8603 // Possible null reference return.
			return qualifiedName;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}
}
