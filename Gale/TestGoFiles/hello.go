package main

import "fmt"
func main(){
    var myarr [2]int
    myarr[1] = 12
    var fib int = fibonachy(10)
    var i int = 1;
    for i <= 10{
        fmtPrintln(i)
        i = i + 1
    }
    var cnter int = myarr[1] + 1
    fmtPrintln(cnter)
    if 2 > 4 {
        fmtPrintln("uno1-if")
    } else {
        fmtPrintln("uno1-else")
    }

    var uno int = 2 + 3 - 1 + 300
    fmtPrintln("uno")
    fmtPrintln(uno)
    var f, h int = 32, 33
    f, h = 12, 13
    fmtPrintln(uno + f + h)
}

func get19() int {
    return 19
}

func fibonachy(n int) int {
    var res int = n;
    if (n > 1){
        var f1 int = fibonachy(n - 1)
        var f2 int = fibonachy(n - 2)
        fmtPrintln("FIBS")
        fmtPrintln(f1)
        fmtPrintln(f2)
        res = f1+f2
    }
    return res;
    //return fibonachy(f1, f1+f0, n)
}

func recursionTest(counter int) int {
    
    fmtPrintln(counter)
    fmtPrintln(counter <= 1)
    if (counter <= 1){
        return get19()
    }

    counter = counter - 1;
    return recursionTest(counter)
}