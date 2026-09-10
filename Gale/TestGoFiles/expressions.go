package main

import "fmt"

func main(){
    

    var uno int = 2 + 3 - 1
    var f, h int = 12, 33
    f, h = 12, 13
    //f = 13 + 2
    //fmt.Println(2 + 3)
    //var f float = 1.3 + 1
    hui()
    var g int = 2 + (3 - 4) // для каждого выражения нужно каким-то образом определять возвращаемый тип
    //
    var gg int = g

    //c, f, m := 1, "honk", 'f'
    //b := 3 // ИДЕНТИФИКАТОР ПРИСВАИВАНИЕ ЗНАЧЕНИЕ
    //a := 2 + b // ИДЕНТИФИКАТОР ПРИСВАИВАНИЕ ЗНАЧЕНИЕ ОПЕРАНД ИДЕНТИФИКАТОР
    fmt.Println(gg) // ВЫЗОВ ФУНКЦИИ
}

func hui() int {
    return 34;
}