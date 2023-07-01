void main () {
    int arr[4];
    int i;
    i = 0;
    arr[0] = 0;
    arr[1] = 10;
    arr[2] = 20;
    arr[3] = 30;

    
    ++arr[++i];
    print i;
    print arr[0];
    print arr[1];
    print arr[2];
    print arr[3];
}