Feature: SpecTest
	In order to show how spec flow works
	As a simple example
	I want to click a button on the screen

@mytag
Scenario: Press a button
    Given I start the app with some <capabilities>
	And I have waiting for the app to have loaded
	And the element named <Button> is visible
	When I click the element named <Button>
	Then the element named <Image> is visible
    And I save a screenshot
