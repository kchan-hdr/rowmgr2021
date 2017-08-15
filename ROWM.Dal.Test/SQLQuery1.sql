insert into rowm.Ownership( parcelid, ownerid, ownership_t, created, LastModified, ModifiedBy )
select pid, o.OwnerId, 1, '2017-8-1', '2017-8-1', 'seed'
from Ownership_import i
inner join rowm.owner o on i.PartyName = o.PartyName

select * from rowm.Ownership