-- function newCounter()
--     local count = 0
--     return function ()
--         count = count + 1
--         return count
--     end
-- end

-- c1 = newCounter()
-- print(c1())
-- print(c1())

-- c2 = newCounter()
-- print(c2())
-- print(c1())
-- print(c2())

function f1(n)
    local function f2()
        print(n)
    end
    n = n + 10
    return f2
end

g1 = f1(1979)
g1()
