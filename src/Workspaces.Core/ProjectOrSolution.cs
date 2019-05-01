// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal readonly struct ProjectOrSolution : IEquatable<ProjectOrSolution>
    {
        private readonly Project _project;
        private readonly Solution _solution;

        internal ProjectOrSolution(Project project)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
            _solution = null;
        }

        internal ProjectOrSolution(Solution solution)
        {
            _solution = solution ?? throw new ArgumentNullException(nameof(solution));
            _project = null;
        }

        public bool IsProject => _project != null;

        public bool IsSolution => _solution != null;

        public bool IsDefault => _project == null && _solution == null;

        public string FilePath => (IsProject) ? _project.FilePath : _solution?.FilePath;

        public VersionStamp Version => (IsProject) ? _project.Version : (_solution?.Version ?? default);

        public Workspace Workspace => (IsProject) ? _project.Solution.Workspace : _solution?.Workspace;

        public Project AsProject() => _project;

        public Solution AsSolution() => _solution;

        public override string ToString()
        {
            return (_project ?? (object)_solution)?.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is ProjectOrSolution other
                && Equals(other);
        }

        public bool Equals(ProjectOrSolution other)
        {
            if (_project != null)
            {
                return _project == other._project;
            }
            else if (_solution != null)
            {
                return _solution == other._solution;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (_project ?? (object)_solution)?.GetHashCode() ?? 0;
        }

        public static bool operator ==(in ProjectOrSolution left, in ProjectOrSolution right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(in ProjectOrSolution left, in ProjectOrSolution right)
        {
            return !left.Equals(right);
        }

        public static implicit operator ProjectOrSolution(Project project)
        {
            return new ProjectOrSolution(project);
        }

        public static implicit operator Project(in ProjectOrSolution ifOrElse)
        {
            return ifOrElse.AsProject();
        }

        public static implicit operator ProjectOrSolution(Solution solution)
        {
            return new ProjectOrSolution(solution);
        }

        public static implicit operator Solution(in ProjectOrSolution ifOrElse)
        {
            return ifOrElse.AsSolution();
        }
    }
}
