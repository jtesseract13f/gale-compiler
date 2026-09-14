package main

import "fmt"
import "container/list"

type Person struct {
    Name string
    Age  int
}

func main(){
    var p Person = Person{"Анна", 30}
    //var p Person = Person{Name: "Анна", Age: 30}
    var mylist *list.List = list.New()
    var ints []int = []int{10, 2, 85, 41, 5}
    
    l := list.New()
    //animal.Name + 3
    fmt.Println(mylist, l)

    //Print("hui")
    //hive.getAnimal().Tail.Tailed[1:3].PrintTailed()[2:6]
    //
}
