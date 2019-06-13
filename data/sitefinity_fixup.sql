update [sf_mb_dynamic_module_field]
set related_data_provider = 'sf-site-default-provider'
where module_name = 'Discover Your Skills And Careers' and nme = 'Skill'

update [sf_meta_attribute]
set val = 'sf-site-default-provider'
where id2 = '3FC82677-87DB-4816-9835-E23FD5A1E2DA' and nme = 'RelatedProviders'

select * from sf_meta_attribute
where id2 = '3FC82677-87DB-4816-9835-E23FD5A1E2DA'

select * from sf_mb_dynamic_module_field fld
inner join sf_mb_dynamic_module mod on mod.nme = fld.module_name
where mod.nme = 'Discover Your Skills And Careers'