select * from AspNetUsers --where id = 'f273825e-4386-43ad-849e-06d22f0d5c45'

delete from AspNetUsers where id in ('df7f7921-d60e-4201-bc7e-9ea372e38a21', '251ce3cb-9540-446b-9f59-6f5c33d1f102');

update AspNetUsers
set UserName = 'Ivan'
where id = '37f3f96e-dac6-4ce7-9765-7be793458309';