// https://on.cypress.io/api

describe('My First Test', () => {
  it('visits the app root url', () => {
    cy.viewport(1920, 1080)
    cy.visit('/login')

    cy.contains('Account').invoke('attr', 'for')
    .then((id) => {
      cy.get('#' + id).type('allen')
    });

    cy.contains('Password').invoke('attr', 'for')
    .then((id) => {
      cy.get('#' + id).type('12345678')
    });

    cy.pause();

    cy.get("button.bg-success").click();
  })
})
