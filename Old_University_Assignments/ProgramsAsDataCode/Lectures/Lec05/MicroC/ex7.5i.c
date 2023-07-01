// micro-C example 7.5i
// not finished

void main() {
    int whocares[4];
    int res;
    whocares[0] = 7;
    whocares[1] = 13;
    whocares[2] = 9;
    whocares[3] = 8;
    arrsum(4, whocares, &res);
    print res;
    println;
}

void arrsum (int n, int arr[], int *sump){
    int i;
    *sump = 0;
    for (i = 0; i < n; ++i) *sump = *sump +arr[i];
}
