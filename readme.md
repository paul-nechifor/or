# This repository has been moved to [gitlab.com/paul-nechifor/or-homework](http://gitlab.com/paul-nechifor/or-homework).

Old readme:

Operational Research Homework
=============================

## Problems

- [T1C-e](#t1c-e)
- [T1C-A3](#t1c-a3)
- [T1C-B3](#t1c-b3)
- [T1C-c1](#t1c-c1)
- [T1C-c2](#t1c-c2)
- [T1D-e](#t1d-e)
- [T1D-1](#t1d-1)
- [T1D-2](#t1d-2)
- [T1E-e](#t1e-e)
- [T1E-A3](#t1e-a3)
- [T1E-1](#t1e-1)
- [T1E-2](#t1e-2)
- [T2A](#t2a)
- [T2B-a](#t2b-a)
- [T2B-b-test](#t2b-b-test)
- [T2B-b](#t2b-b)
- [T2B-c-test](#t2b-c-test)
- [T2B-c](#t2b-c)
- [T2C-test1](#t2c-test1)
- [T2C-test2](#t2c-test2)

### T1C-e

```
    A = [[0.5, -5.5, -2.5, 9], [0.5, -1.5, -0.5, 1], [1, 0, 0, 0]];
    b = [0, 0, 1];
    c = [10, -57, -9, -24];
    x = simplex(A, b, c);
    equal(x, [1, 0, 1, 0]);
```

### T1C-A3

```
    A = [[0.2, 0.3], [0.4, 0.3]];
    b = [14, 16];
    c = [20, 25];
    x = simplex(A, b, c);
    equal(x, [10, 40]);
```

### T1C-B3

```
    g = [[0, 1, 3], [0, 2, 6], [0, 3, 2], [1, 2, 2], [2, 3, 5], [1, 4, 4], [1, 5, 2], [2, 5, 3], [3, 5, 3], [3, 6, 4], [4, 5, 4], [5, 6, 2], [4, 7, 4], [6, 7, 5]];
    x = simplexMaxFlow(g, 0, 7);
    equal(x, [3, 3, 2, 0, 2, 3, 0, 1, 0, 4, 0, 1, 3, 5]);
```

### T1C-c1

```
    A = [[1, 1, 2], [2, 2, 1], [2, 3, 0]];
    b = [6, 8, 6];
    c = [2, 1, 3];
    x = simplex(A, b, c);
    equal(x, [3, 0, 1.5]);
```

### T1C-c2

```
    A = [[1, 1, 1, 1], [1, -11, -5, 18], [1, -3, -1, 2]];
    b = [1, 0, 0];
    c = [-1, 7, 1, 2];
    x = simplex(A, b, c);
    equal(x, [0, 1, 0, 0]);
```

### T1D-e

```
    A = [[2, 1, 1], [1, -1, 0], [1, 0, -1]];
    b = [4, -1, -2];
    c = [1, 1, -2];
    x = simplex(A, b, c);
    equal(x, [0, 2, 2]);
```

### T1D-1

```
    A = [[1, -3, 1], [2, 2, -1], [2, 1, -2]];
    b = [-1, 4, 2];
    c = [2, -2, 1];
    x = simplex(A, b, c);
```

### T1D-2

```
    A = [[1, 2, -1, 0], [2, 0, 1, 1], [1, 0, 0, -1]];
    b = [-2, 10, -1];
    c = [1, 0, 3, -2];
    x = simplex(A, b, c);
    equal(x, [0, 3.5, 9, 1]);
```

### T1E-e

```
    A = [[1, -1], [-2, -1], [-1, 2]];
    b = [-1, -4, 0];
    c = [2, -3];
    y = simplexDual(A, b, c);
    equal(y, [1, 2]);
```

### T1E-A3

```
    A = [[-0.4, -0.3], [-0.2, -0.3]];
    b = [-16, -14];
    c = [-20, -25];
    y = simplexDual(A, b, c);
    equal(y, [10, 40]);
```

### T1E-1

```
    A = [[-1, -1, -2], [-2, -2, -1], [-2, -3, 0]];
    b = [-6, -8, -6];
    c = [-2, -1, -3];
    y = simplexDual(A, b, c);
    equal(y, [3, 0, 1.5]);
```

### T1E-2

```
    A = [[-1, -1, -1, -1], [-1, 11, 5, -18], [-1, 3, 1, -2]];
    b = [-1, 0, 0];
    c = [1, -7, -1, -2];
    y = simplexDual(A, b, c);
    equal(y, [0, 1, 0, 0]);
```

### T2A

```
    A = [[0.5, -5.5, -2.5, 9], [0.5, -1.5, -0.5, 1], [1, 0, 0, 0]];
    b = [0, 0, 1];
    c = [10, -57, -9, -24];
    x = simplex2(A, b, c);
    equal(x, [1, 0, 1, 0]);
```

```
    A = [[0.2, 0.3], [0.4, 0.3]];
    b = [14, 16];
    c = [20, 25];
    x = simplex2(A, b, c);
    equal(x, [10, 40]);
```

### T2B-a

```
    A1 = [[1, 1, 2], [2, 2, 1], [2, 3, 0]];
    b1 = [6, 8, 6];
    c1 = [2, 1, 3];
    p1 = simplexProblem(A1, b1, c1);
    x1 = simplex2(p1);
    
    A2 = [[0.5, 1.5, 2], [2.5, 3, 1], [2, 2.5, 0]];
    b2 = [5, 9, 6.5];
    c2 = [2.5, 1, 2.5];
    p2 = simplexProblem(A2, b2, c2);
    x2 = simplex2(p2);
    
    t1 = simplex2TableauSolved(p1);
    x2res = simplex2Restart(t1, p2);
    
    f21b(t1, b1, b2);
```

### T2B-b-test

```
    A = [[1, 2, 1], [3, 0, 2], [1, 4, 0]];
    b = [430, 460, 420];
    c = [3, 2, 5];
    t = simplex2TableauSolved(simplexProblem(A, b, c));
    withSelf = f21b(t, b, b);
    example = f21b(t, b, [1, 1, 1]);
```

### T2B-b

```
    A1 = [[1, 1, 2], [2, 2, 1], [2, 3, 0]];
    b1 = [6, 8, 6]; 
    c1 = [2, 1, 3];
    t1 = simplex2TableauSolved(simplexProblem(A1, b1, c1));
    b2 = [5, 9, 6.5];
    initialLimits = f21b(t1, b1, b1);
    alteredLimits = f21b(t1, b1, b2);
```

### T2B-c-test

```
    A = [[1, 2, 1], [3, 0, 2], [1, 4, 0]];
    b = [430, 460, 420];
    c = [3, 2, 5];
    t = simplex2TableauSolved(simplexProblem(A, b, c));
    f21c(t, c, c);
    f21c(t, c, [999, -999, 999]);
```

### T2B-c

```
    A1 = [[1, 1, 2], [2, 2, 1], [2, 3, 0]];
    b1 = [6, 8, 6]; 
    c1 = [2, 1, 3];
    t1 = simplex2TableauSolved(simplexProblem(A1, b1, c1));
    c2 = [2.5, 1, 2.5];
    initialLimits = f21c(t1, c1, c1);
    alteredLimits = f21c(t1, c1, c2);
```

### T2C-test1

```
    A = [[2, 1, 0, 0], [4, 3, -1, 0], [1, 2, 0, 1]];
    b = [3, 6, 4];
    c = [4, 1, 0, 0];
    x = simplex3(A, b, c);
```

### T2C-test2

```
    A = [[1, 2, 1], [3, 0, 2], [1, 4, 0]];
    b = [230, 260, 220];
    c = [3, 2, 5];
    x = simplex2(A, b, c);
    x = f22c(A, b, c, [2, 1], [50, 130]);
```
