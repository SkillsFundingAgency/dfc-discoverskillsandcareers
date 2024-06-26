/****** Script for SelectTopNRows command from SSMS  ******/
SELECT DISTINCT TRIM(' ' FROM jp.Title) Title, jp.AlternativeTitle, jp.SalaryStarter, jp.SalaryExperienced, jp.MinimumHours, jp.MaximumHours, workingPatternTax.title_, workingPatternDtlsTax.title_
  FROM jobprofile_jobprofile jp
  INNER JOIN jbprfl_jbprfle_working_pattern workpattern on jp.base_id = workpattern.base_id
  INNER JOIN sf_taxa workingPatternTax on workingPatternTax.id = workpattern.val
  INNER JOIN jbprfl_jbprfl_wrkng_pttrn_dtls workpatternDtls on jp.base_id = workpatternDtls.base_id
  INNER JOIN sf_taxa workingPatternDtlsTax on workingPatternDtlsTax.id = workpatternDtls.val
where SalaryStarter = 0 or SalaryExperienced = 0
order by Title