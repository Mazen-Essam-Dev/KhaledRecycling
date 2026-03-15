# CompareService Documentation

## Overview
`CompareService` is a service class that provides string comparison functionality.  
It implements the `ICompareService` interface and mainly offers:

1. **Levenshtein Distance Calculation** – Measures the minimum number of single-character edits required to change one string into another.
2. **Similarity Percentage** – Calculates how similar two strings are as a percentage based on Levenshtein distance.

---

## Methods

### 1. LevenshteinDistance

```csharp
- public async Task<int> LevenshteinDistance(string s, string t)
```

```
# Description:
Calculates the Levenshtein distance between two strings s and t.
This distance represents the minimum number of single-character insertions, deletions, or substitutions required to change one string into the other.

## Parameters:
s : The first string to compare.
t : The second string to compare.

## Returns:
int – The computed distance between the two strings.

## Implementation Details:
Initializes a 2D array d of size (n+1, m+1) where n and m are the lengths of s and t.
Sets up base cases where one of the strings is empty.

## Iterates through each character of both strings:
Calculates the cost of substitution (0 if characters are equal, 1 otherwise).

## Updates the distance matrix using the minimum of:
Deletion
Insertion
Substitution
```

### 2. SimilarityPercentage
```csharp
-public async Task<double> SimilarityPercentage(string? s, string? t)
```

```
#Description:
Calculates the similarity percentage between two strings s and t based on the Levenshtein distance.

## Parameters:
s : First string to compare (nullable).
t : Second string to compare (nullable).

## Returns:
double – A value between 0 and 100 representing how similar the strings are.

## Implementation Details:
### Returns 0 if either string is null.
### Computes the Levenshtein distance between s and t.
### Using For Calculates Similarity
```