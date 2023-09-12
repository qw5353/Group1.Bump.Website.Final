describe('Register', () => {
    it('測試註冊', () => {
      cy.viewport(1920, 1080)
      cy.visit('/register')
  
      cy.contains('帳號').invoke('attr', 'for')
      .then((id) => {
        cy.get('#' + id).type('leojudya')
      });
  
      cy.contains('信箱').invoke('attr', 'for')
      .then((id) => {
        cy.get('#' + id).type('leojudya@gmail.com')
      });

      cy.contains('密碼').invoke('attr', 'for')
      .then((id) => {
        cy.get('#' + id).type('12345678')
      });

      cy.contains('確認密碼').invoke('attr', 'for')
      .then((id) => {
        cy.get('#' + id).type('12345678')
      });
  
  
      cy.pause();
  
      cy.get('.card-buttom > .v-card-actions > .v-btn > .v-btn__content').click()

      cy.pause();

      cy.get('.v-card-actions > :nth-child(3)').click();


    })
  })
  