describe('GoogleRegister', () => {
  it('註冊', () => {
    cy.viewport(1920, 1080)
    cy.visit('/https://localhost:5002/Account/OAuthRegister?Account=waxapple666&Email=waxapple666@gmail.com&Name=Carol%20Yang&Thumbnail=https%3A%2F%2Flh3.googleusercontent.com%2Fa%2FAAcHTte_fkeA1Kkjw1RVwH0LSRlNOTlKO8i8Eq5bfsNpRs9-2Qw%3Ds96-c')

    cy.contains('暱稱').invoke('attr', 'for')
      .then((id) => {
        cy.get('#' + id).type('caroro')
      });

    cy.contains('手機').invoke('attr', 'for')
      .then((id) => {
        cy.get('#' + id).type('0915110201')
      });

    cy.pause();

  })
})
