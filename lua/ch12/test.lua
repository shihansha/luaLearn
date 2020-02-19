t = { a = 1, b = 2, c = 3, "a", "b" }
for k, v in pairs(t) do
    print(k, v)
end

print()

t = { "a", "b", "c", a = 1, b = 2 }
for k, v in ipairs(t) do
    print(k, v)
end
