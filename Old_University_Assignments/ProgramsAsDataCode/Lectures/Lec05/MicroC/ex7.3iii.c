// micro-C example 7.3iii

void main() { 
    int freq[4];
    int arr[7];
    arr[0] = 1;
    arr[1] = 2;
    arr[2] = 1;
    arr[3] = 1;
    arr[4] = 1;
    arr[5] = 2;
    arr[6] = 0;
    int max;
    max = 3;
    
    int i;

    histogram(7, arr, max, freq);
    
    for (i = 0; i <= max; i = i + 1) print freq[i];
}


void histogram(int n, int ns[], int max, int freq[]){
    int i;

    for (i = 0; i <= max; i = i + 1) freq[i] = 0;

    for (i = 0; i < n; i = i + 1) {
        int j;
        j = ns[i];
        if (j <= max) freq[j] = freq[j] + 1;
  }
}