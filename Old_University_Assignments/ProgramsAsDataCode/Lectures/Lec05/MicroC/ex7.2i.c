// micro-C example 7.2i

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
    int sum;
    sum = 0;
    int i;
    i = 0;
    while(i < n){
       sum = sum + arr[i];
       i = i+1;
    }
    *sump = sum;
}
