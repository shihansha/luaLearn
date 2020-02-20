print("hello world! \
\
good job!")
local b = [[
this is a long
long
long string.
]]
local h = [==[
[this is [[a strange string]]]
]==]
--[===[
    this is a strange comment
]===]
--[[
    this is a comment
    comment
]]
local c, d, e, f, g = .123, 12., 0x32.4P4, 32.e12 -- comment
function dummy(a)
    if a == 1 or a == 2 then
        return 1
    end
    
    return dummy(a - 1) + dummy(a - 2);
end
print(dummy(6))