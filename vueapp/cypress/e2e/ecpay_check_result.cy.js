describe('Check ECPay', () => {
    it('查詢結果', () => {
      cy.viewport(1920, 1080)
      cy.visit('https://vendor-stage.ecpay.com.tw/')
        
      cy.get('#ecpayLogin').click()

      cy.get('#Account').type("stagetest3")
      cy.get('#LoginAllPay').click()
      cy.get('#Password').type('test1234')
      cy.get('#AuthNO').type('00000000')

      cy.pause()

      cy.get('#LoginAllPay').click()
    })
  })
  