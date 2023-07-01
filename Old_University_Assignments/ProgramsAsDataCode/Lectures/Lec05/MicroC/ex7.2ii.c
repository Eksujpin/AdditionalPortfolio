// micro-C example 7.2ii

void main(int n) {
    int funcsq[20];
    int sum;
    if (n <= 20){
        squares(n, funcsq);
        arrsum(n, funcsq, &sum);
        print sum;
    }
}

void squares (int n, int arr[]){
    int i;
    i = 0;
    while (i < n){
        arr[i] = i*i;
        i = i+1;
    } 
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