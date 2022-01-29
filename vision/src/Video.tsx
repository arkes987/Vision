import {
  HttpTransportType,
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";
import { useEffect, useState } from "react";
import { Col, Container, Row } from "react-bootstrap";
import { IMessage } from "./IMessage";

export const Video = () => {
  const [connection, setConnection] = useState<HubConnection>();
  const [streamOne, setStreamOne] = useState<string>();
  const [streamTwo, setStreamTwo] = useState<string>();

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      .withUrl("http://77.87.73.205:9880/hubs/imagestream", {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Debug)
      .build();

    setConnection(newConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then((result) => {
          console.log("Connected!");

          connection.on("ReceiveMessage", (message: IMessage) => {
            switch (message.channel) {
              case "First":
                setStreamOne(message.file);
                break;
              case "Second":
                setStreamTwo(message.file);
                break;
            }
          });
        })
        .catch((e) => console.log("Connection failed: ", e));
    }
  }, [connection]);

  return (
    <Container>
      <Row>
        <Col>
          <img src={`data:image/png;base64,${streamOne}`} alt="Cam" />
        </Col>
        <Col>
          <img src={`data:image/png;base64,${streamTwo}`} alt="Cam" />
        </Col>
      </Row>
    </Container>
  );
};
