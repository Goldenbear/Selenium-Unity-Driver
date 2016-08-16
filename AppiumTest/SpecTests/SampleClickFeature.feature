Feature: SampleFeature
	In order to show how spec flow works
	As a simple example
	I want to click a button on the screen

@onAndroid @onIOS
Scenario Outline: Press a button
    Given I start the app with driver type <capabilities>
	And I have waited for the app to have loaded
	And the element named <Button> is visible
	When I click the element named <Button>
	Then the element named <Image> is visible
    And I save a screenshot

    Examples: 
	| Description | capabilities  | Button | Image |
	| Android     | SampleAndroid | Button | Image |
	| iOS         | SampleIOS     | Button | Image | 
