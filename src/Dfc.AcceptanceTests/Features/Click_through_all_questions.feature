Feature: click_through_all_questions
	In order to test end-to-end
	As a user
	I want to click-through the options to reach 100%

Background: 
	Given Set up test


	@smoke_test
	Scenario Outline: Strongly Agree only click through assessment and filter questions and return a set of jobs
	Given I have openned DYSAC main page
	When I have selected to start assessment
	Then I am on question page 

	#Leader

	Then I click on answer <Leader>
	Then I click on answer <Leader>
	Then I click on answer <Leader>
	Then I click on answer <Leader>
	Then I click on answer <Leader>
	#Driver
	Then I click on answer <Driver>
	Then I click on answer <Driver>
	Then I click on answer <Driver>
	Then I click on answer <Driver>
	Then I click on answer <Driver>
	#Influencer
	Then I click on answer <Influencer>
	Then I click on answer <Influencer>
	Then I click on answer <Influencer>
	Then I click on answer <Influencer>
	Then I click on answer <Influencer>
	#Helper
	Then I click on answer <Helper>
	Then I click on answer <Helper>
	Then I click on answer <Helper>
	Then I click on answer <Helper>
	Then I click on answer <Helper>
	#Analyst
	Then I click on answer <Analyst>
	Then I click on answer <Analyst>
	Then I click on answer <Analyst>
	Then I click on answer <Analyst>
	Then I click on answer <Analyst>
	#Creator
	Then I click on answer <Creator>
	Then I click on answer <Creator>
	Then I click on answer <Creator>
	Then I click on answer <Creator>
	Then I click on answer <Creator>
	#Organiser
	Then I click on answer <Organiser>
	Then I click on answer <Organiser>
	Then I click on answer <Organiser>
	Then I click on answer <Organiser>
	Then I click on answer <Organiser>
	#Doer
	Then I click on answer <Doer>
	Then I click on answer <Doer>
	Then I click on answer <Doer>
	Then I click on answer <Doer>
	Then I click on answer <Doer>

	Then I am on Assessment Complete Page
	And I click on see results
	Then I am on the your results page
	And There is an option to print my results

	And some job categories are returned


	When I click on the Filter Questions Button
	Then I am shown a filter question page
	
	#test return button
	When I click on Return Button
	Then I am returned to the Results Page
	When I click on the reloaded Filter Questions Button

	# Sports Jobs - 3 Filter Questions Test->
	Then I am shown a filter question page

	When I click on the filter Yes button
	And click on the filter next question button
	
	And I click on the filter Yes button
	And click on the filter next question button

	And I click on the filter Yes button
	And click on the filter next question button

	Then I am on Assessment Complete Page
	And I click on see results
	#Then I am on the your results page

	And I am shown Job Roles returned by filter question criteria


	And I teardown the test

	Examples:
	| Leader | Driver | Influencer | Helper | Analyst | Creator | Organiser | Doer |
	| 1      | 1      | 1          | 1      | 1       | 1       | 1         | 1    |


	@smoke_test
	Scenario Outline: Strongly Disagree only click through to assessment complete to show no jobs
	Given I have openned DYSAC main page
	When I have selected to start assessment
	Then I am on question page 

	#Leader

	Then I click on answer <Leader>
	Then I click on answer <Leader>
	Then I click on answer <Leader>
	Then I click on answer <Leader>
	Then I click on answer <Leader>
	#Driver
	Then I click on answer <Driver>
	Then I click on answer <Driver>
	Then I click on answer <Driver>
	Then I click on answer <Driver>
	Then I click on answer <Driver>
	#Influencer
	Then I click on answer <Influencer>
	Then I click on answer <Influencer>
	Then I click on answer <Influencer>
	Then I click on answer <Influencer>
	Then I click on answer <Influencer>
	#Helper
	Then I click on answer <Helper>
	Then I click on answer <Helper>
	Then I click on answer <Helper>
	Then I click on answer <Helper>
	Then I click on answer <Helper>
	#Analyst
	Then I click on answer <Analyst>
	Then I click on answer <Analyst>
	Then I click on answer <Analyst>
	Then I click on answer <Analyst>
	Then I click on answer <Analyst>
	#Creator
	Then I click on answer <Creator>
	Then I click on answer <Creator>
	Then I click on answer <Creator>
	Then I click on answer <Creator>
	Then I click on answer <Creator>
	#Organiser
	Then I click on answer <Organiser>
	Then I click on answer <Organiser>
	Then I click on answer <Organiser>
	Then I click on answer <Organiser>
	Then I click on answer <Organiser>
	#Doer
	Then I click on answer <Doer>
	Then I click on answer <Doer>
	Then I click on answer <Doer>
	Then I click on answer <Doer>
	Then I click on answer <Doer>


	Then I am on Assessment Complete Page
	And I click on see results
	Then I am on the your results page
	
	And no Job Categories are found message shown

	And I teardown the test

	Examples:
	| Leader | Driver | Influencer | Helper | Analyst | Creator | Organiser | Doer |
	| 5     | 5      | 5          | 5      | 5       | 5       | 5         | 5    |





	@smoke_test
	Scenario Outline: It depends only click through to assesment complete to show no jobs
	Given I have openned DYSAC main page
	When I have selected to start assessment
	Then I am on question page 

	#Leader

	Then I click on answer <Leader>
	Then I click on answer <Leader>
	Then I click on answer <Leader>
	Then I click on answer <Leader>
	Then I click on answer <Leader>
	#Driver
	Then I click on answer <Driver>
	Then I click on answer <Driver>
	Then I click on answer <Driver>
	Then I click on answer <Driver>
	Then I click on answer <Driver>
	#Influencer
	Then I click on answer <Influencer>
	Then I click on answer <Influencer>
	Then I click on answer <Influencer>
	Then I click on answer <Influencer>
	Then I click on answer <Influencer>
	#Helper
	Then I click on answer <Helper>
	Then I click on answer <Helper>
	Then I click on answer <Helper>
	Then I click on answer <Helper>
	Then I click on answer <Helper>
	#Analyst
	Then I click on answer <Analyst>
	Then I click on answer <Analyst>
	Then I click on answer <Analyst>
	Then I click on answer <Analyst>
	Then I click on answer <Analyst>
	#Creator
	Then I click on answer <Creator>
	Then I click on answer <Creator>
	Then I click on answer <Creator>
	Then I click on answer <Creator>
	Then I click on answer <Creator>
	#Organiser
	Then I click on answer <Organiser>
	Then I click on answer <Organiser>
	Then I click on answer <Organiser>
	Then I click on answer <Organiser>
	Then I click on answer <Organiser>
	#Doer
	Then I click on answer <Doer>
	Then I click on answer <Doer>
	Then I click on answer <Doer>
	Then I click on answer <Doer>
	Then I click on answer <Doer>

	Then I am on Assessment Complete Page
	And I click on see results
	Then I am on the your results page
	
	And no Job Categories are found message shown

	And I teardown the test

	Examples:
	| Leader | Driver | Influencer | Helper | Analyst | Creator | Organiser | Doer |
	| 3      | 3      | 3          | 3      | 3       | 3       | 3         | 3    |

	@smoke_test
	Scenario Outline: Negative and depends only click through to assessment complete to show no jobs
	Given I have openned DYSAC main page
	When I have selected to start assessment
	Then I am on question page 

	#Leader
	Then I click on answer <answer4>
	Then I click on answer <answer3>
	Then I click on answer <answer3>
	#Negative
	Then I click on answer <answer2> 
	Then I click on answer <answer5>

	#Driver
	Then I click on answer <answer4>
	Then I click on answer <answer5>
	Then I click on answer <answer3>
	Then I click on answer <answer4>
	Then I click on answer <answer5>

	#Influencer
	Then I click on answer <answer3>
	Then I click on answer <answer4>
	Then I click on answer <answer3>
	Then I click on answer <answer4>
	#Negative
	Then I click on answer <answer2>

	#Helper
	Then I click on answer <answer3>
	Then I click on answer <answer4>
	Then I click on answer <answer3>
	Then I click on answer <answer4>
	Then I click on answer <answer5>

	#Analyst
	Then I click on answer <answer5>
	Then I click on answer <answer3>
	Then I click on answer <answer3>
	Then I click on answer <answer4>
	Then I click on answer <answer5>

	#Creator
	Then I click on answer <answer4>
	Then I click on answer <answer4>
	Then I click on answer <answer3>
	Then I click on answer <answer4>
	Then I click on answer <answer5>

	#Organiser
	Then I click on answer <answer4>
	Then I click on answer <answer4>
	Then I click on answer <answer3>
	Then I click on answer <answer4>
	#Negative
	Then I click on answer <answer1>

	#Doer
	Then I click on answer <answer4>
	Then I click on answer <answer4>
	Then I click on answer <answer3>
	Then I click on answer <answer4>
	#Negative
	Then I click on answer <answer1>

	Then I am on Assessment Complete Page
	And I click on see results
	Then I am on the your results page
	
	And no Job Categories are found message shown

	And I teardown the test

	Examples:
	| answer1 | answer2 | answer3 | answer4 | answer5 |
	| 1       | 2       | 3       | 4       | 5       |





	@smoke_test
	Scenario Outline: Issue and use reference number after assessment complete
	Given I have openned DYSAC main page
	When I have selected to start assessment
	Then I am on question page 

	#Leader
	Then I click on answer <answer1>
	Then I click on answer <answer2>
	Then I click on answer <answer2>
	Then I click on answer <answer1>
	Then I click on answer <answer2>

	#Driver
	Then I click on answer <answer1>
	Then I click on answer <answer1>
	Then I click on answer <answer2>
	Then I click on answer <answer3>
	Then I click on answer <answer3>

	#Influencer
	Then I click on answer <answer1>
	Then I click on answer <answer2>
	Then I click on answer <answer3>
	Then I click on answer <answer3>
	Then I click on answer <answer3>

	#Helper
	Then I click on answer <answer1>
	Then I click on answer <answer2>
	Then I click on answer <answer3>
	Then I click on answer <answer3>
	Then I click on answer <answer3>

	#Analyst
	Then I click on answer <answer1>
	Then I click on answer <answer2>
	Then I click on answer <answer3>
	Then I click on answer <answer3>
	Then I click on answer <answer3>

	#Creator
	Then I click on answer <answer1>
	Then I click on answer <answer2>
	Then I click on answer <answer3>
	Then I click on answer <answer2>
	Then I click on answer <answer1>

	#Organiser
	Then I click on answer <answer1>
	Then I click on answer <answer2>
	Then I click on answer <answer3>
	Then I click on answer <answer3>
	Then I click on answer <answer3>

	#Doer
	Then I click on answer <answer1>
	Then I click on answer <answer2>
	Then I click on answer <answer2>
	Then I click on answer <answer2>
	Then I click on answer <answer5>

	Then I am on Assessment Complete Page
	And I click on see results
	Then I am on the your results page


	#work in progress - ref nos
	And I click on send results button 
	And I choose option to generate reference number
	Then I am presented with a reference number

	And I click to return to assessment from reference number page
	And I am on Results Page

	When I re-open the DYSAC main start page
	And enter my reference number
	Then I am on the your results page


	And I teardown the test

	Examples:
	| answer1 | answer2 | answer3 | answer4 | answer5 |
	| 1       | 2       | 3       | 4       | 5       |





	@adhoc_test
	Scenario Outline: adhoc combinations test for editing that leaves browser open
	Given I have openned DYSAC main page
	When I have selected to start assessment
	Then I am on question page 

	#Leader

	Then I click on answer <Leader>
	Then I click on answer <Leader>
	Then I click on answer <Leader>
	Then I click on answer <Leader>
	Then I click on answer <Leader>
	#Driver
	Then I click on answer <Driver>
	Then I click on answer <Driver>
	Then I click on answer <Driver>
	Then I click on answer <Driver>
	Then I click on answer <Driver>
	#Influencer
	Then I click on answer <Influencer>
	Then I click on answer <Influencer>
	Then I click on answer <Influencer>
	Then I click on answer <Influencer>
	Then I click on answer <Influencer>
	#Helper
	Then I click on answer <Helper>
	Then I click on answer <Helper>
	Then I click on answer <Helper>
	Then I click on answer <Helper>
	Then I click on answer <Helper>
	#Analyst
	Then I click on answer <Analyst>
	Then I click on answer <Analyst>
	Then I click on answer <Analyst>
	Then I click on answer <Analyst>
	Then I click on answer <Analyst>
	#Creator
	Then I click on answer <Creator>
	Then I click on answer <Creator>
	Then I click on answer <Creator>
	Then I click on answer <Creator>
	Then I click on answer <Creator>
	#Organiser
	Then I click on answer <Organiser>
	Then I click on answer <Organiser>
	Then I click on answer <Organiser>
	Then I click on answer <Organiser>
	Then I click on answer <Organiser>
	#Doer
	Then I click on answer <Doer>
	Then I click on answer <Doer>
	Then I click on answer <Doer>
	Then I click on answer <Doer>
	Then I click on answer <Doer>

	Then I am on Assessment Complete Page

	And I teardown the test

	Examples:
	| Leader | Driver | Influencer | Helper | Analyst | Creator | Organiser | Doer |
	| 3      | 1      | 3          | 3      | 3       | 3      | 1         | 1    |


#
#
#	@adhoc_test
#	Scenario Outline: Driver only click through to assessment complete
#	Given I have openned DYSAC main page
#	When I have selected to start assessment
#	Then I am on question page 
#
#	#Leader
#
#	Then I click on answer <Leader>
#	Then I click on answer <Leader>
#	Then I click on answer <Leader>
#	Then I click on answer <Leader>
#	Then I click on answer <Leader>
#	#Driver
#	Then I click on answer <Driver>
#	Then I click on answer <Driver>
#	Then I click on answer <Driver>
#	Then I click on answer <Driver>
#	Then I click on answer <Driver>
#	#Influencer
#	Then I click on answer <Influencer>
#	Then I click on answer <Influencer>
#	Then I click on answer <Influencer>
#	Then I click on answer <Influencer>
#	Then I click on answer <Influencer>
#	#Helper
#	Then I click on answer <Helper>
#	Then I click on answer <Helper>
#	Then I click on answer <Helper>
#	Then I click on answer <Helper>
#	Then I click on answer <Helper>
#	#Analyst
#	Then I click on answer <Analyst>
#	Then I click on answer <Analyst>
#	Then I click on answer <Analyst>
#	Then I click on answer <Analyst>
#	Then I click on answer <Analyst>
#	#Creator
#	Then I click on answer <Creator>
#	Then I click on answer <Creator>
#	Then I click on answer <Creator>
#	Then I click on answer <Creator>
#	Then I click on answer <Creator>
#	#Organiser
#	Then I click on answer <Organiser>
#	Then I click on answer <Organiser>
#	Then I click on answer <Organiser>
#	Then I click on answer <Organiser>
#	Then I click on answer <Organiser>
#	#Doer
#	Then I click on answer <Doer>
#	Then I click on answer <Doer>
#	Then I click on answer <Doer>
#	Then I click on answer <Doer>
#	Then I click on answer <Doer>
#
#	Then I am on Assessment Complete Page
#
#	And I teardown the test
#
#	Examples:
#	| Leader | Driver | Influencer | Helper | Analyst | Creator | Organiser | Doer |
#	| 1      | 5      | 2          | 2      | 5       | 5       | 5         | 5    |
#
#
##	@smoke_test
#	Scenario Outline: Agree only click through to assessment complete
#	Given I have openned DYSAC main page
#	When I have selected to start assessment
#	Then I am on question page 
#
#	#Leader
#
#	Then I click on answer <Leader>
#	Then I click on answer <Leader>
#	Then I click on answer <Leader>
#	Then I click on answer <Leader>
#	Then I click on answer <Leader>
#	#Driver
#	Then I click on answer <Driver>
#	Then I click on answer <Driver>
#	Then I click on answer <Driver>
#	Then I click on answer <Driver>
#	Then I click on answer <Driver>
#	#Influencer
#	Then I click on answer <Influencer>
#	Then I click on answer <Influencer>
#	Then I click on answer <Influencer>
#	Then I click on answer <Influencer>
#	Then I click on answer <Influencer>
#	#Helper
#	Then I click on answer <Helper>
#	Then I click on answer <Helper>
#	Then I click on answer <Helper>
#	Then I click on answer <Helper>
#	Then I click on answer <Helper>
#	#Analyst
#	Then I click on answer <Analyst>
#	Then I click on answer <Analyst>
#	Then I click on answer <Analyst>
#	Then I click on answer <Analyst>
#	Then I click on answer <Analyst>
#	#Creator
#	Then I click on answer <Creator>
#	Then I click on answer <Creator>
#	Then I click on answer <Creator>
#	Then I click on answer <Creator>
#	Then I click on answer <Creator>
#	#Organiser
#	Then I click on answer <Organiser>
#	Then I click on answer <Organiser>
#	Then I click on answer <Organiser>
#	Then I click on answer <Organiser>
#	Then I click on answer <Organiser>
#	#Doer
#	Then I click on answer <Doer>
#	Then I click on answer <Doer>
#	Then I click on answer <Doer>
#	Then I click on answer <Doer>
#	Then I click on answer <Doer>

#	Then I am on Assessment Complete Page
#	And I click on see results
#	Then I am on the your results page
#	And There is an option to print my results
#
#	And I teardown the test
#
#	Examples:
#	| Leader | Driver | Influencer | Helper | Analyst | Creator | Organiser | Doer |
#	| 1      | 2      | 1          | 1      | 2       | 2       | 2         | 2    |


#
#
#
#	@adhoc_test
#	Scenario Outline: Negative Doer question - EAM should be returned too
#	Given I have openned DYSAC main page
#	When I have selected to start assessment
#	Then I am on question page 
#
#	#Leader
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#
#	#Driver
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#
#	#Influencer
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#
#	#Helper
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#
#	#Analyst
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#
#	#Creator
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#
#	#Organiser
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#
#	#Doer
#	Then I click on answer <answer1>
# 
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer3>
#	Then I click on answer <answer1>
#
#	Then I am on Assessment Complete Page
#	And I teardown the test
#
#	Examples:
#	| answer1 | answer2 | answer3 | answer4 | answer5 |
#	| 1       | 2       | 3       | 4       | 5       |
#
#
#
#	@adhoc_test
#	Scenario Outline: driver leader assessment complete
#	Given I have openned DYSAC main page
#	When I have selected to start assessment
#	Then I am on question page
#	#Leader
#
#	Then I click on answer <Leader>
#	Then I click on answer <Leader>
#	Then I click on answer <Leader>
#	Then I click on answer <Leader>
#	Then I click on answer <Leader>
#	#Driver
#	Then I click on answer <Driver>
#	Then I click on answer <Driver>
#	Then I click on answer <Driver>
#	Then I click on answer <Driver>
#	Then I click on answer <Driver>
#	#Influencer
#	Then I click on answer <Influencer>
#	Then I click on answer <Influencer>
#	Then I click on answer <Influencer>
#	Then I click on answer <Influencer>
#	Then I click on answer <Influencer>
#	#Helper
#	Then I click on answer <Helper>
#	Then I click on answer <Helper>
#	Then I click on answer <Helper>
#	Then I click on answer <Helper>
#	Then I click on answer <Helper>
#	#Analyst
#	Then I click on answer <Analyst>
#	Then I click on answer <Analyst>
#	Then I click on answer <Analyst>
#	Then I click on answer <Analyst>
#	Then I click on answer <Analyst>
#	#Creator
#	Then I click on answer <Creator>
#	Then I click on answer <Creator>
#	Then I click on answer <Creator>
#	Then I click on answer <Creator>
#	Then I click on answer <Creator>
#	#Organiser
#	Then I click on answer <Organiser>
#	Then I click on answer <Organiser>
#	Then I click on answer <Organiser>
#	Then I click on answer <Organiser>
#	Then I click on answer <Organiser>
#	#Doer
#	Then I click on answer <Doer>
#	Then I click on answer <Doer>
#	Then I click on answer <Doer>
#	Then I click on answer <Doer>
#	Then I click on answer <Doer>
#
#	Then I am on Assessment Complete Page
#
#	And I teardown the test
#
#	Examples:
#	| Leader | Driver | Influencer | Helper | Analyst | Creator | Organiser | Doer |
#	| 1      | 1      | 3          | 3      | 3       | 3      | 3         | 3    |
#
#
#	@adhoc_test
#	Scenario Outline: helper assessment complete
#	Given I have openned DYSAC main page
#	When I have selected to start assessment
#	Then I am on question page 
#
#	#Leader
#
#	Then I click on answer <Leader>
#	Then I click on answer <Leader>
#	Then I click on answer <Leader>
#	Then I click on answer <Leader>
#	Then I click on answer <Leader>
#	#Driver
#	Then I click on answer <Driver>
#	Then I click on answer <Driver>
#	Then I click on answer <Driver>
#	Then I click on answer <Driver>
#	Then I click on answer <Driver>
#	#Influencer
#	Then I click on answer <Influencer>
#	Then I click on answer <Influencer>
#	Then I click on answer <Influencer>
#	Then I click on answer <Influencer>
#	Then I click on answer <Influencer>
#	#Helper
#	Then I click on answer <Helper>
#	Then I click on answer <Helper>
#	Then I click on answer <Helper>
#	Then I click on answer <Helper>
#	Then I click on answer <Helper>
#	#Analyst
#	Then I click on answer <Analyst>
#	Then I click on answer <Analyst>
#	Then I click on answer <Analyst>
#	Then I click on answer <Analyst>
#	Then I click on answer <Analyst>
#	#Creator
#	Then I click on answer <Creator>
#	Then I click on answer <Creator>
#	Then I click on answer <Creator>
#	Then I click on answer <Creator>
#	Then I click on answer <Creator>
#	#Organiser
#	Then I click on answer <Organiser>
#	Then I click on answer <Organiser>
#	Then I click on answer <Organiser>
#	Then I click on answer <Organiser>
#	Then I click on answer <Organiser>
#	#Doer
#	Then I click on answer <Doer>
#	Then I click on answer <Doer>
#	Then I click on answer <Doer>
#	Then I click on answer <Doer>
#	Then I click on answer <Doer>
#
#	Then I am on Assessment Complete Page
#
#	And I teardown the test
#
#	Examples:
#	| Leader | Driver | Influencer | Helper | Analyst | Creator | Organiser | Doer |
#	| 3      | 3      | 3          | 2      | 3       | 3      | 3         | 3    |
#
#
#
#
#
#	Scenario Outline: equal positive and negative for each trait group 
#	Given I have openned DYSAC main page
#	When I have selected to start assessment
#	Then I am on question page 
#
#	#Leader
#	Then I click on answer <answer1>
#	Then I click on answer <answer2>
#	Then I click on answer <answer3>
#	#Negative
#	Then I click on answer <answer2> 
#	Then I click on answer <answer5>
#
#	#Driver
#	Then I click on answer <answer1>
#	Then I click on answer <answer2>
#	Then I click on answer <answer3>
#	Then I click on answer <answer4>
#	Then I click on answer <answer5>
#
#	#Influencer
#	Then I click on answer <answer1>
#	Then I click on answer <answer2>
#	Then I click on answer <answer3>
#	Then I click on answer <answer4>
#	#Negative
#	Then I click on answer <answer1>
#
#	#Helper
#	Then I click on answer <answer1>
#	Then I click on answer <answer2>
#	Then I click on answer <answer3>
#	Then I click on answer <answer4>
#	Then I click on answer <answer5>
#
#	#Analyst
#	Then I click on answer <answer1>
#	Then I click on answer <answer2>
#	Then I click on answer <answer3>
#	Then I click on answer <answer4>
#	Then I click on answer <answer5>
#
#	#Creator
#	Then I click on answer <answer1>
#	Then I click on answer <answer2>
#	Then I click on answer <answer3>
#	Then I click on answer <answer4>
#	Then I click on answer <answer5>
#
#	#Organiser
#	Then I click on answer <answer1>
#	Then I click on answer <answer2>
#	Then I click on answer <answer3>
#	Then I click on answer <answer4>
#	#Negative
#	Then I click on answer <answer1>
#
#	#Doer
#	Then I click on answer <answer1>
#	Then I click on answer <answer2>
#	Then I click on answer <answer3>
#	Then I click on answer <answer4>
#	#Negative
#	Then I click on answer <answer1>
#
#	Then I am on Assessment Complete Page
#	And I teardown the test
#
#	Examples:
#	| answer1 | answer2 | answer3 | answer4 | answer5 |
#	| 1       | 2       | 3       | 4       | 5       |
#
#	
