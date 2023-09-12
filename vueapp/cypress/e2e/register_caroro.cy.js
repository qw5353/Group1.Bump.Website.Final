describe('Register2', () => {
    it('測試註冊2', () => {
      cy.viewport(1920, 1080)
      cy.visit('/register')
  
      cy.contains('帳號').invoke('attr', 'for')
        .then((id) => {
          cy.get('#' + id).type('caroro')
        });
  
      cy.contains('信箱').invoke('attr', 'for')
        .then((id) => {
          cy.get('#' + id).type('caroroyang@gmail.com')
        });
  
      cy.contains('密碼').invoke('attr', 'for')
        .then((id) => {
          cy.get('#' + id).type('123456789')
        });
  
      cy.contains('確認密碼').invoke('attr', 'for')
        .then((id) => {
          cy.get('#' + id).type('123456789')
        });
  
  
      cy.pause();
  
      // cy.get('.card-buttom > .v-card-actions > .v-btn > .v-btn__content').click()
  
      // cy.pause();
  
      cy.contains('名字').invoke('attr', 'for')
        .then((id) => {
          cy.get('#' + id).type('Carol Yang')
        });
  
      cy.contains('暱稱').invoke('attr', 'for')
        .then((id) => {
          cy.get('#' + id).type('caroro')
        });
  
      // cy.contains('性別').invoke('attr', 'for')
      //   .then((id) => {
      //     cy.get('#' + id).type('女性')
      //   });
  
      // cy.contains('生日').invoke('attr', 'for')
      //   .then((id) => {
      //     cy.get('#' + id).type('2000-01-01')
      //   });
  
      cy.contains('手機').invoke('attr', 'for')
        .then((id) => {
          cy.get('#' + id).type('0900110200')
        });
  
    })
  })