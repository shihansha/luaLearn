print("hello world!")
function dummy(a)
    if a == 1 or a == 2 then
        return 1
    end
    
    return dummy(a - 1) + dummy(a - 2);
end
print(dummy(6))