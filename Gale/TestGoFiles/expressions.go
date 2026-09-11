package main

import "fmt"
func main(){
    var myarr [2]int
    myarr[1] = 0
    var cnter int = recursionTest(3)
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

func recursionTest(counter int) int {
    
    fmtPrintln(counter)
    fmtPrintln(counter <= 1)
    if (counter <= 1){
        return get19()
    }

    counter = counter - 1;
    return recursionTest(counter)
}